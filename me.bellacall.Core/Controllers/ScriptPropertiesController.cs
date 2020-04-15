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
    [SwaggerTag("<code>Кампании &rarr; Сценарии &rarr; Свойства (статические переменные)</code>")]
    public class ScriptPropertiesController : ApiController<ScriptProperty, ScriptPropertyModel>
    {
        public ScriptPropertiesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override ScriptProperty GetEntity(ScriptPropertyModel model)
        {
            return new ScriptProperty
            {
                Id = model.Id,
                Script_Id = model.Script_Id,
                Name = model.Name,
                DataType = model.DataType,
                Value = model.Value
            };
        }

        protected override ScriptPropertyModel GetModel(ScriptProperty entity)
        {
            return new ScriptPropertyModel
            {
                Id = entity.Id,
                Script_Id = entity.Script_Id,
                Name = entity.Name,
                DataType = entity.DataType,
                Value = entity.Value
            };
        }

        /// <summary>
        /// Возвращает список свойств
        /// </summary>
        /// <param name="script_Id">SCRIPT_ID свойства</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptPropertys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScriptPropertyModel>>> GetScriptPropertys([FromQuery(Name = "oid")] long[] script_Id)
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
        /// Возвращает свойство
        /// </summary>
        /// <param name="id">ID свойства</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/ScriptPropertys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ScriptPropertyModel>> GetScriptProperty(long id)
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
        /// Обновляет свойство
        /// </summary>
        /// <param name="id">ID свойства</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/ScriptPropertys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScriptProperty(long id, ScriptPropertyModel model)
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
        /// Добавляет свойство
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/ScriptPropertys
        [HttpPost]
        public async Task<ActionResult<ScriptPropertyModel>> PostScriptProperty(ScriptPropertyCreateModel model)
        {
            var campaign = DB.Scripts.Find(model.Script_Id)?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, campaign.Id, GetModel(entity));

            return CreatedAtAction(nameof(GetScriptProperty), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет свойство
        /// </summary>
        /// <param name="id">ID свойства</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/ScriptPropertys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScriptProperty(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            var campaign = entity?.Script?.Campaign;

            var result = Check(campaign is Campaign, NotFound).OkNull() ?? Check(DB.Scripts, Operation.Update, campaign.Id);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, campaign.Id, GetModel(entity));

            return NoContent();
        }
    }
}
