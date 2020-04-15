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
    [SwaggerTag("<code>Кампании &rarr; Сценарии &rarr; Элементы</code>")]
    public class ScriptElementsController : ApiController<ScriptElement, ScriptElementModel>
    {
        public ScriptElementsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ScriptElement GetEntity(ScriptElementModel model)
        {
            return new ScriptElement
            {
                Id = model.Id,
                Script_Id = model.Script_Id,
                Name = model.Name,
                Description = model.Description,
                Function = model.Function,
                XPos = model.XPos,
                YPos = model.YPos
            };
        }

        protected override ScriptElementModel GetModel(ScriptElement entity)
        {
            return new ScriptElementModel
            {
                Id = entity.Id,
                Script_Id = entity.Script_Id,
                Name = entity.Name,
                Description = entity.Description,
                Function = entity.Function,
                XPos = entity.XPos,
                YPos = entity.YPos
            };
        }

        /// <summary>
        /// Возвращает список элементов
        /// </summary>
        /// <param name="script_Id">SCRIPT_ID элемента</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptElements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptElementModel>>> GetScriptElements([FromQuery(Name = "oid")] long[] script_Id)
        {
            var result = Check(DB.Scripts, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => script_Id.Contains(e.Script_Id))
                .Join(AllowedIds(Operation.Read), o => o.Script.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает элемент
        /// </summary>
        /// <param name="id">ID элемента</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptElements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptElementModel>> GetScriptElement(long id)
        {
            var result = Check(DB.Scripts, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.Script.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет элемента
        /// </summary>
        /// <param name="id">ID элемента</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ScriptElements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScriptElement(long id, ScriptElementModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.Scripts.Find(model.Script_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет элемент
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ScriptElements
        [HttpPost]
        public async Task<ActionResult<ScriptElementModel>> PostScriptElement(ScriptElementCreateModel model)
        {
            var campaign = DB.Scripts.Find(model.Script_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetScriptElement), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет элемент
        /// </summary>
        /// <param name="id">ID элемента</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ScriptElements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScriptElement(long id)
        {
            var entity = await DB_TABLE.Include(e => e.ScriptConditions).Include(e => e.ScriptInputParameters).Include(e => e.ScriptOutputParameters).FirstOrDefaultAsync(e => e.Id == id);
            var campaign = entity?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.ScriptConditions);
                DB.RemoveRange(entity.ScriptInputParameters);
                DB.RemoveRange(entity.ScriptOutputParameters);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, campaign.Id, GetModel(entity));
                Get<ScriptConditionsController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptConditions);
                Get<ScriptInputParametersController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptInputParameters);
                Get<ScriptOutputParametersController>().Log(Operation.Delete, operation_Id, campaign.Id, entity.ScriptOutputParameters);
            }

            return NoContent();
        }
    }
}
