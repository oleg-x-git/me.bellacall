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
    [SwaggerTag("<code>Кампании &rarr; Сценарии &rarr; Элементы &rarr; Входные параметры</code>")]
    public class ScriptInputParametersController : ApiController<ScriptInputParameter, ScriptParameterModel>
    {
        public ScriptInputParametersController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ScriptInputParameter GetEntity(ScriptParameterModel model)
        {
            return new ScriptInputParameter
            {
                Id = model.Id,
                ScriptElement_Id = model.ScriptElement_Id,
                Name = model.Name,
                DataType = model.DataType,
                Expression = model.Expression
            };
        }

        protected override ScriptParameterModel GetModel(ScriptInputParameter entity)
        {
            return new ScriptParameterModel
            {
                Id = entity.Id,
                ScriptElement_Id = entity.ScriptElement_Id,
                Name = entity.Name,
                DataType = entity.DataType,
                Expression = entity.Expression
            };
        }

        /// <summary>
        /// Возвращает список входных параметров
        /// </summary>
        /// <param name="scriptElement_Id">SCRIPTELEMENT_ID входного параметра</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptInputParameters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptParameterModel>>> GetScriptInputParameters([FromQuery(Name = "oid")] long[] scriptElement_Id)
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
        /// Возвращает входной параметр
        /// </summary>
        /// <param name="id">ID входного параметра</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptInputParameters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptParameterModel>> GetScriptInputParameter(long id)
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
        /// Обновляет входной параметр
        /// </summary>
        /// <param name="id">ID входного параметра</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ScriptInputParameters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScriptInputParameter(long id, ScriptParameterModel model)
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
        /// Добавляет входной параметр
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ScriptInputParameters
        [HttpPost]
        public async Task<ActionResult<ScriptParameterModel>> PostScriptInputParameter(ScriptParameterCreateModel model)
        {
            var campaign = DB.ScriptElements.Find(model.ScriptElement_Id)?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetScriptInputParameter), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет входной параметр
        /// </summary>
        /// <param name="id">ID входного параметра</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ScriptInputParameters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScriptInputParameter(long id)
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
