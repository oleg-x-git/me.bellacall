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
    [SwaggerTag("<code>Кампании &rarr; Группы контактов &rarr; Контакты</code>")]
    public class ContactsController : ApiController<Contact, ContactModel>
    {
        public ContactsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Contact GetEntity(ContactModel model)
        {
            return new Contact
            {
                Id = model.Id,
                ContactGroup_Id = model.ContactGroup_Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                Phone = model.Phone,
                Email = model.Email,
                Properties = model.Properties,
                Status = model.Status
            };
        }

        protected override ContactModel GetModel(Contact entity)
        {
            return new ContactModel
            {
                Id = entity.Id,
                ContactGroup_Id = entity.ContactGroup_Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Patronymic = entity.Patronymic,
                BirthDate = entity.BirthDate,
                Gender = entity.Gender,
                Phone = entity.Phone,
                Email = entity.Email,
                Properties = entity.Properties,
                Status = entity.Status
            };
        }

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="contactGroup_Id">CONTACTGROUP_ID контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetContacts([FromQuery(Name = "oid")] long[] contactGroup_Id)
        {
            var result = Check(DB.ContactGroups, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => contactGroup_Id.Contains(e.ContactGroup_Id))
                .Join(AllowedIds(Operation.Read), o => o.ContactGroup.Campaign_Id, i => i, (o, i) => o)
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
        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactModel>> GetContact(long id)
        {
            var result = Check(DB.ContactGroups, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.ContactGroup.Campaign_Id, i => i, (o, i) => o)
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
        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(long id, ContactModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.ContactGroups.Find(model.ContactGroup_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.ContactGroups, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
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
        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult<ContactModel>> PostContact(ContactCreateModel model)
        {
            var campaign = DB.ContactGroups.Find(model.ContactGroup_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.ContactGroups, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetContact), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="id">ID контакта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(long id)
        {
            var entity = await DB_TABLE.Include(e => e.JobContacts).Include(e => e.LeadContacts).FirstOrDefaultAsync(e => e.Id == id);
            var campaign = entity?.ContactGroup?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.ContactGroups, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.JobContacts);
                DB.RemoveRange(entity.LeadContacts);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign.Id, GetModel(entity));
                Get<JobContactsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.JobContacts);
                Get<LeadContactsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.LeadContacts);
            }

            return NoContent();
        }
    }
}
