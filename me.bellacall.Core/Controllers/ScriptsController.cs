using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using me.bellacall.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code>Кампании &rarr; Сценарии</code>")]
    public class ScriptsController : ApiController<Script, ScriptModel>
    {
        public ScriptsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Script GetEntity(ScriptModel model)
        {
            return new Script
            {
                Id = model.Id,
                Campaign_Id = model.Campaign_Id,
                Name = model.Name,
                Description = model.Description
            };
        }

        protected override ScriptModel GetModel(Script entity)
        {
            return new ScriptModel
            {
                Id = entity.Id,
                Campaign_Id = entity.Campaign_Id,
                Name = entity.Name,
                Description = entity.Description
            };
        }

        /// <summary>
        /// Возвращает список сценариев
        /// </summary>
        /// <param name="campaign_Id">CAMPAIGN_ID сценария</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Scripts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptModel>>> GetScripts([FromQuery(Name = "oid")] long[] campaign_Id)
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
        /// Возвращает сценарий
        /// </summary>
        /// <param name="id">ID сценария</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Scripts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptModel>> GetScript(long id)
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
        /// Обновляет сценарий
        /// </summary>
        /// <param name="id">ID сценария</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Scripts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScript(long id, ScriptModel model)
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
        /// Добавляет сценарий
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Scripts
        [HttpPost]
        public async Task<ActionResult<ScriptModel>> PostScript(ScriptCreateModel model)
        {
            var campaign = DB.Campaigns.Find(model.Campaign_Id);

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetScript), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет сценарий
        /// </summary>
        /// <param name="id">ID сценария</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Scripts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScript(long id)
        {
            var entity = await DB_TABLE
                .Include(e => e.ScriptElements).ThenInclude(e => e.ScriptConditions)
                .Include(e => e.ScriptElements).ThenInclude(e => e.ScriptInputParameters)
                .Include(e => e.ScriptElements).ThenInclude(e => e.ScriptOutputParameters)
                .Include(e => e.ScriptVariables)
                .Include(e => e.ScriptProperties)
                .FirstOrDefaultAsync(e => e.Id == id);
            var campaign = entity?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(Operation.Delete, campaign.Id);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.ScriptElements.SelectMany(e => e.ScriptConditions));
                DB.RemoveRange(entity.ScriptElements.SelectMany(e => e.ScriptInputParameters));
                DB.RemoveRange(entity.ScriptElements.SelectMany(e => e.ScriptOutputParameters));
                DB.RemoveRange(entity.ScriptElements);
                DB.RemoveRange(entity.ScriptVariables);
                DB.RemoveRange(entity.ScriptProperties);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign.Id, GetModel(entity));
                Get<ScriptConditionsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptElements.SelectMany(e => e.ScriptConditions));
                Get<ScriptInputParametersController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptElements.SelectMany(e => e.ScriptInputParameters));
                Get<ScriptOutputParametersController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptElements.SelectMany(e => e.ScriptOutputParameters));
                Get<ScriptElementsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptElements);
                Get<ScriptVariablesController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptVariables);
                Get<ScriptPropertiesController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptProperties);
            }

            return NoContent();
        }
    }
}
