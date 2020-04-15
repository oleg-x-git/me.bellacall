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
    [SwaggerTag("<code style='color:gray'>Тарифы</code>")]
    public class TariffsController : ApiController<Tariff, TariffModel>
    {
        public TariffsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Tariff GetEntity(TariffModel model)
        {
            return new Tariff
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                AllowEnjoy = model.AllowEnjoy
            };
        }

        protected override TariffModel GetModel(Tariff entity)
        {
            return new TariffModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                AllowEnjoy = entity.AllowEnjoy
            };
        }

        /// <summary>
        /// Возвращает список тарифов
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Tariffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TariffModel>>> GetTariffs()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает тариф
        /// </summary>
        /// <param name="id">ID тарифа</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Tariffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TariffModel>> GetTariff(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет тариф
        /// </summary>
        /// <param name="id">ID тарифа</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Tariffs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTariff(long id, TariffModel model)
        {
            var result = Check(id == model.Id, BadRequest).OkNull() ?? Check(Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет тариф
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Tariffs
        [HttpPost]
        public async Task<ActionResult<TariffModel>> PostTariff(TariffCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetTariff), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет тариф
        /// </summary>
        /// <param name="id">ID тарифа</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Tariffs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTariff(long id)
        {
            var entity = await DB_TABLE.Include(e => e.TariffConditions).FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            var result = Check(Operation.Delete);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.TariffConditions);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, null, GetModel(entity));
                Get<TariffConditionsController>().Log(Operation.Delete, operation_Id, null, entity.TariffConditions);
            }

            return NoContent();
        }
    }
}
