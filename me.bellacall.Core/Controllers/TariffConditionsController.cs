using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using me.bellacall.Core.Data.Common;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code style='color:gray'>Тарифы &rarr; Условия</code>")]
    public class TariffConditionsController : ApiController<TariffCondition, TariffConditionModel>
    {
        public TariffConditionsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override TariffCondition GetEntity(TariffConditionModel model)
        {
            return new TariffCondition
            {
                Id = model.Id,
                Tariff_Id = model.Tariff_Id,
                DateStart = model.DateStart,
                DateStop = model.DateStop,
                Rule = model.Rule,
                Unit = model.Unit,
                Price = model.Price
            };
        }

        protected override TariffConditionModel GetModel(TariffCondition entity)
        {
            return new TariffConditionModel
            {
                Id = entity.Id,
                Tariff_Id = entity.Tariff_Id,
                DateStart = entity.DateStart,
                DateStop = entity.DateStop,
                Rule = entity.Rule,
                Unit = entity.Unit,
                Price = entity.Price
            };
        }

        /// <summary>
        /// Возвращает список условий
        /// </summary>
        /// <param name="tariff_Id">TARIFF_ID условия</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/TariffConditions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TariffConditionModel>>> GetTariffConditions([FromQuery(Name = "oid")] long[] tariff_Id)
        {
            var result = Check(DB.Tariffs, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(entity => tariff_Id.Contains(entity.Tariff_Id))
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает условие
        /// </summary>
        /// <param name="id">ID условия</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/TariffConditions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TariffConditionModel>> GetTariffCondition(long id)
        {
            var result = Check(DB.Tariffs, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет условия
        /// </summary>
        /// <param name="id">ID условия</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/TariffConditions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTariffCondition(long id, TariffConditionModel model)
        {
            var result = Check(id == model.Id, BadRequest).OkNull() ?? Check(DB.Tariffs, Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет условие
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/TariffConditions
        [HttpPost]
        public async Task<ActionResult<TariffConditionModel>> PostTariffCondition(TariffConditionCreateModel model)
        {
            var result = Check(DB.Tariffs, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetTariffCondition), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет условие
        /// </summary>
        /// <param name="id">ID условия</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/TariffConditions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTariffCondition(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            if (entity == null) return NotFound();

            var result = Check(DB.Tariffs, Operation.Update);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));

            return NoContent();
        }
    }
}
