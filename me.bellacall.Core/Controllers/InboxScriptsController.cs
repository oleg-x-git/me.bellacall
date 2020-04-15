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
    [SwaggerTag("<code>Кампании &rarr; Рассылки &rarr; Сценарии коллбэка</code>")]
    public class InboxScriptsController : ApiController<InboxScript, InboxScriptModel>
    {
        public InboxScriptsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override InboxScript GetEntity(InboxScriptModel model)
        {
            return new InboxScript
            {
                Id = model.Id,
                Job_Id = model.Job_Id,
                Script_Id = model.Script_Id
            };
        }

        protected override InboxScriptModel GetModel(InboxScript entity)
        {
            return new InboxScriptModel
            {
                Id = entity.Id,
                Job_Id = entity.Job_Id,
                Script_Id = entity.Script_Id
            };
        }

        /// <summary>
        /// Возвращает список сценариев коллбэка
        /// </summary>
        /// <param name="job_Id">JOB_ID сценария коллбэка</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/InboxScripts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InboxScriptModel>>> GetInboxScripts([FromQuery(Name = "oid")] long[] job_Id)
        {
            var result = Check(DB.Jobs, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => job_Id.Contains(e.Job_Id))
                .Join(AllowedIds(Operation.Read), o => o.Job.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает сценарий коллбэка
        /// </summary>
        /// <param name="id">ID сценария коллбэка</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/InboxScripts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InboxScriptModel>> GetInboxScript(long id)
        {
            var result = Check(DB.Jobs, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.Job.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет сценарий коллбэка
        /// </summary>
        /// <param name="id">ID сценария коллбэка</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/InboxScripts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInboxScript(long id, InboxScriptModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Jobs.Find(model.Job_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Jobs, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет сценарий коллбэка
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/InboxScripts
        [HttpPost]
        public async Task<ActionResult<InboxScriptModel>> PostInboxScript(InboxScriptCreateModel model)
        {
            var campaign = DB.Jobs.Find(model.Job_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Jobs, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetInboxScript), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет сценарий коллбэка
        /// </summary>
        /// <param name="id">ID сценария коллбэка</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/InboxScripts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInboxScript(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.Job?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Jobs, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
