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
    [SwaggerTag("<code>Кампании &rarr; Лиды &rarr; Контакты</code>")]
    public class LeadContactsController : ApiController<LeadContact, LeadContactModel>
    {
        public LeadContactsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override LeadContact GetEntity(LeadContactModel model)
        {
            return new LeadContact
            {
                Id = model.Id,
                Lead_Id = model.Lead_Id,
                Contact_Id = model.Contact_Id
            };
        }

        protected override LeadContactModel GetModel(LeadContact entity)
        {
            return new LeadContactModel
            {
                Id = entity.Id,
                Lead_Id = entity.Lead_Id,
                Contact_Id = entity.Contact_Id
            };
        }

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="lead_Id">LEAD_ID контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/LeadContacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeadContactModel>>> GetLeadContacts([FromQuery(Name = "oid")] long[] lead_Id)
        {
            var result = Check(DB.Leads, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => lead_Id.Contains(e.Lead_Id))
                .Join(AllowedIds(Operation.Read), o => o.Lead.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает контакт
        /// </summary>
        /// <param name="id">ID контакта</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/LeadContacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeadContactModel>> GetLeadContact(long id)
        {
            var result = Check(DB.Leads, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.Lead.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет контакт
        /// </summary>
        /// <param name="id">ID контакта</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/LeadContacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeadContact(long id, LeadContactModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Leads.Find(model.Lead_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Leads, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет контакт
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/LeadContacts
        [HttpPost]
        public async Task<ActionResult<LeadContactModel>> PostLeadContact(LeadContactCreateModel model)
        {
            var campaign = DB.Leads.Find(model.Lead_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Leads, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetLeadContact), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="id">ID контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/LeadContacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadContact(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.Lead?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Leads, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
