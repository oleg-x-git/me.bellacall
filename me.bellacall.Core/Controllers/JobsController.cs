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
    [SwaggerTag("<code>Кампании &rarr; Рассылки</code>")]
    public class JobsController : ApiController<Job, JobModel>
    {
        public JobsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Job GetEntity(JobModel model)
        {
            return new Job
            {
                Id = model.Id,
                Campaign_Id = model.Campaign_Id,

                // .. Параметры рассылки ..
                Name = model.Name,
                Region_Id = model.Region_Id,
                AllowJob = model.AllowJob,
                AllowInbox = model.AllowInbox,
                TimeStart = model.TimeStart,
                TimeStop = model.TimeStop,
                TimeZoneType = model.TimeZoneType,
                TimeZoneCustom = model.TimeZoneCustom,

                // .. Параметры дозвона ..
                DialDuration = model.DialDuration,
                DialDensity = model.DialDensity,
                DialEfforts = model.DialEfforts,
                DialInterval = model.DialInterval
            };
        }

        protected override JobModel GetModel(Job entity)
        {
            return new JobModel
            {
                Id = entity.Id,
                Campaign_Id = entity.Campaign_Id,

                // .. Параметры рассылки ..
                Name = entity.Name,
                Region_Id = entity.Region_Id,
                AllowJob = entity.AllowJob,
                AllowInbox = entity.AllowInbox,
                TimeStart = entity.TimeStart,
                TimeStop = entity.TimeStop,
                TimeZoneType = entity.TimeZoneType,
                TimeZoneCustom = entity.TimeZoneCustom,

                // .. Параметры дозвона ..
                DialDuration = entity.DialDuration,
                DialDensity = entity.DialDensity,
                DialEfforts = entity.DialEfforts,
                DialInterval = entity.DialInterval
            };
        }

        /// <summary>
        /// Возвращает список рассылок
        /// </summary>
        /// <param name="campaign_Id">CAMPAIGN_ID рассылки</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetJobs([FromQuery(Name = "oid")] long[] campaign_Id)
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
        /// Возвращает рассылку
        /// </summary>
        /// <param name="id">ID рассылки</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobModel>> GetJob(long id)
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
        /// Обновляет рассылку
        /// </summary>
        /// <param name="id">ID рассылки</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Jobs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(long id, JobModel model)
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
        /// Добавляет рассылку
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Jobs
        [HttpPost]
        public async Task<ActionResult<JobModel>> PostJob(JobCreateModel model)
        {
            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetJob), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет рассылку
        /// </summary>
        /// <param name="id">ID рассылки</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(long id)
        {
            var entity = await DB_TABLE.Include(e => e.JobContacts).Include(e => e.JobScripts).Include(e => e.InboxScripts).FirstOrDefaultAsync(e => e.Id == id);
            var campaign = entity?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Delete, campaign.Id);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.JobContacts);
                DB.RemoveRange(entity.JobScripts);
                DB.RemoveRange(entity.InboxScripts);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign.Id, GetModel(entity));
                Get<JobContactsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.JobContacts);
                Get<JobScriptsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.JobScripts);
                Get<InboxScriptsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.InboxScripts);
            }

            return NoContent();
        }
    }
}
