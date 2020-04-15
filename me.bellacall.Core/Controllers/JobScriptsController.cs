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
    [SwaggerTag("<code>Кампании &rarr; Рассылки &rarr; Сценарии рассылки</code>")]
    public class JobScriptsController : ApiController<JobScript, JobScriptModel>
    {
        public JobScriptsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override JobScript GetEntity(JobScriptModel model)
        {
            return new JobScript
            {
                Id = model.Id,
                Job_Id = model.Job_Id,
                Script_Id = model.Script_Id
            };
        }

        protected override JobScriptModel GetModel(JobScript entity)
        {
            return new JobScriptModel
            {
                Id = entity.Id,
                Job_Id = entity.Job_Id,
                Script_Id = entity.Script_Id
            };
        }

        /// <summary>
        /// Возвращает список сценариев рассылки
        /// </summary>
        /// <param name="job_Id">JOB_ID сценария рассылки</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/JobScripts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobScriptModel>>> GetJobScripts([FromQuery(Name = "oid")] long[] job_Id)
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
        /// Возвращает сценарий рассылки
        /// </summary>
        /// <param name="id">ID сценария рассылки</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/JobScripts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobScriptModel>> GetJobScript(long id)
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
        /// Обновляет сценарий рассылки
        /// </summary>
        /// <param name="id">ID сценария рассылки</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/JobScripts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobScript(long id, JobScriptModel model)
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
        /// Добавляет сценарий рассылки
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/JobScripts
        [HttpPost]
        public async Task<ActionResult<JobScriptModel>> PostJobScript(JobScriptCreateModel model)
        {
            var campaign = DB.Jobs.Find(model.Job_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Jobs, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetJobScript), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет сценарий рассылки
        /// </summary>
        /// <param name="id">ID сценария рассылки</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/JobScripts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobScript(long id)
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
