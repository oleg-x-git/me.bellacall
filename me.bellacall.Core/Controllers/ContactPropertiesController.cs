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
    [SwaggerTag("<code>Кампании &rarr; Свойства контактов</code>")]
    public class ContactPropertiesController : ApiController<ContactProperty, ContactPropertyModel>
    {
        public ContactPropertiesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ContactProperty GetEntity(ContactPropertyModel model)
        {
            return new ContactProperty
            {
                Id = model.Id,
                Campaign_Id = model.Campaign_Id,
                Name = model.Name,
                Caption = model.Caption,
                Type = model.Type
            };
        }

        protected override ContactPropertyModel GetModel(ContactProperty entity)
        {
            return new ContactPropertyModel
            {
                Id = entity.Id,
                Campaign_Id = entity.Campaign_Id,
                Name = entity.Name,
                Caption = entity.Caption,
                Type = entity.Type
            };
        }

        /// <summary>
        /// Возвращает список свойств контакта
        /// </summary>
        /// <param name="campaign_Id">CAMPAIGN_ID свойства контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ContactProperties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactPropertyModel>>> GetContactProperties([FromQuery(Name = "oid")] long[] campaign_Id)
        {
            var result = Check(DB.Campaigns, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => campaign_Id.Contains(e.Campaign_Id))
                .Join(AllowedIds(Operation.Read), o => o.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает свойство контакта
        /// </summary>
        /// <param name="id">ID свойства контакта</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ContactProperties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactPropertyModel>> GetContactProperty(long id)
        {
            var result = Check(DB.Campaigns, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет свойство контакта
        /// </summary>
        /// <param name="id">ID свойства контакта</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ContactProperties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactProperty(long id, ContactPropertyModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Campaigns, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет свойство контакта
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ContactProperties
        [HttpPost]
        public async Task<ActionResult<ContactPropertyModel>> PostContactProperty(ContactPropertyCreateModel model)
        {
            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Campaigns, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetContactProperty), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет свойство контакта
        /// </summary>
        /// <param name="id">ID свойства контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ContactProperties/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactProperty(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Campaigns, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
