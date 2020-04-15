using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code>Кампании &rarr; Группы контактов</code>")]
    public class ContactGroupsController : ApiController<ContactGroup, ContactGroupModel>
    {
        public ContactGroupsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ContactGroup GetEntity(ContactGroupModel model)
        {
            return new ContactGroup
            {
                Id = model.Id,
                Campaign_Id = model.Campaign_Id,
                Name = model.Name,
                Description = model.Description
            };
        }

        protected override ContactGroupModel GetModel(ContactGroup entity)
        {
            return new ContactGroupModel
            {
                Id = entity.Id,
                Campaign_Id = entity.Campaign_Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        /// <summary>
        /// Возвращает список групп контактов
        /// </summary>
        /// <param name="campaign_Id">CAMPAIGN_ID группы контактов</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ContactGroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactGroupModel>>> GetContactGroups([FromQuery(Name = "oid")] long[] campaign_Id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => campaign_Id.Contains(e.Campaign_Id))
                .Join(AllowedIds(Operation.Read), o => o.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает группу контактов
        /// </summary>
        /// <param name="id">ID группы контактов</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ContactGroups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactGroupModel>> GetContactGroup(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет группу контактов
        /// </summary>
        /// <param name="id">ID группы контактов</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ContactGroups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactGroup(long id, ContactGroupModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет группу контактов
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ContactGroups
        [HttpPost]
        public async Task<ActionResult<ContactGroupModel>> PostContactGroup(ContactGroupCreateModel model)
        {
            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetContactGroup), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет группу контактов
        /// </summary>
        /// <param name="id">ID группы контактов</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ContactGroups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactGroup(long id)
        {
            var entity = await DB_TABLE.Include(e => e.Contacts).FirstOrDefaultAsync(e => e.Id == id);
            var campaign = entity?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Delete, campaign.Id);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.Contacts);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign.Id, GetModel(entity));
                Get<ContactsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.Contacts);
            }

            return NoContent();
        }
    }
}
