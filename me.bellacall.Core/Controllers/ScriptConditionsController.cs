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
    [SwaggerTag("<code>Кампании &rarr; Сценарии &rarr; Элементы &rarr; Условные переходы</code>")]
    public class ScriptConditionsController : ApiController<ScriptCondition, ScriptConditionModel>
    {
        public ScriptConditionsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ScriptCondition GetEntity(ScriptConditionModel model)
        {
            return new ScriptCondition
            {
                Id = model.Id,
                ScriptElement_Id = model.ScriptElement_Id,
                Name = model.Name,
                CaseExpression = model.CaseExpression,
                CaseElement_Id = model.CaseElement_Id
            };
        }

        protected override ScriptConditionModel GetModel(ScriptCondition entity)
        {
            return new ScriptConditionModel
            {
                Id = entity.Id,
                ScriptElement_Id = entity.ScriptElement_Id,
                Name = entity.Name,
                CaseExpression = entity.CaseExpression,
                CaseElement_Id = entity.CaseElement_Id
            };
        }

        /// <summary>
        /// Возвращает список условных переходов
        /// </summary>
        /// <param name="scriptElement_Id">SCRIPTELEMENT_ID условного перехода</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptConditions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptConditionModel>>> GetScriptConditions([FromQuery(Name = "oid")] long[] scriptElement_Id)
        {
            var result = Check(DB.Scripts, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => scriptElement_Id.Contains(e.ScriptElement_Id))
                .Join(AllowedIds(Operation.Read), o => o.ScriptElement.Script.Campaign_Id, i => i, (o, i) => o)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает условынй переход
        /// </summary>
        /// <param name="id">ID условного перехода</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptConditions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptConditionModel>> GetScriptCondition(long id)
        {
            var result = Check(DB.Scripts, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Join(AllowedIds(Operation.Read), o => o.ScriptElement.Script.Campaign_Id, i => i, (o, i) => o)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет условный переход
        /// </summary>
        /// <param name="id">ID условного перехода</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ScriptConditions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScriptCondition(long id, ScriptConditionModel model)
        {
            if (id != model.Id) return BadRequest();

            var campaign = DB.ScriptElements.Find(model.ScriptElement_Id)?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update, campaign.Id).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, campaign.Id, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет условный переход
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ScriptConditions
        [HttpPost]
        public async Task<ActionResult<ScriptConditionModel>> PostScriptCondition(ScriptConditionCreateModel model)
        {
            var campaign = DB.ScriptElements.Find(model.ScriptElement_Id)?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetScriptCondition), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет условный переход
        /// </summary>
        /// <param name="id">ID условного перехода</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ScriptConditions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScriptCondition(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.ScriptElement?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
