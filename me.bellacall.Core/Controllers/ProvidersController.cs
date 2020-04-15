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
    [SwaggerTag("<code style='color:gray'>Операторы связи</code>")]
    public class ProvidersController : ApiController<Provider, ProviderModel>
    {
        public ProvidersController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Provider GetEntity(ProviderModel model)
        {
            return new Provider
            {
                Id = model.Id,
                Name = model.Name,
                Code = model.Code
            };
        }

        protected override ProviderModel GetModel(Provider entity)
        {
            return new ProviderModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code
            };
        }

        /// <summary>
        /// Возвращает список операторов связи
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Providers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProviderModel>>> GetProviders()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает оператора связи
        /// </summary>
        /// <param name="id">ID оператора связи</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Providers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProviderModel>> GetProvider(long id)
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
        /// Обновляет оператора связи
        /// </summary>
        /// <param name="id">ID оператора связи</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Providers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProvider(long id, ProviderModel model)
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
        /// Добавляет оператора связи
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Providers
        [HttpPost]
        public async Task<ActionResult<ProviderModel>> PostProvider(ProviderCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetProvider), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет оператора связи
        /// </summary>
        /// <param name="id">ID оператора связи</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Providers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvider(long id)
        {
            var entity = await DB_TABLE.Include(e => e.DEFs).Include(e => e.MNPs).FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            var result = Check(Operation.Delete);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.DEFs);
                DB.RemoveRange(entity.MNPs);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, null, GetModel(entity));
                Get<DEFsController>().Log(Operation.Delete, operation_Id, null, entity.DEFs);
                Get<MNPsController>().Log(Operation.Delete, operation_Id, null, entity.MNPs);
            }

            return NoContent();
        }
    }
}
