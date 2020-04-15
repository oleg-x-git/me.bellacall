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
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code style='color:gray'>Перенесенные мобильные номера</code>")]
    public class MNPsController : ApiController<MNP, MNPModel>
    {
        public MNPsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override MNP GetEntity(MNPModel model)
        {
            return new MNP
            {
                Id = model.Id,
                Number = model.Number,
                Provider_Id = model.Provider_Id,
                Region_Id = model.Region_Id
            };
        }

        protected override MNPModel GetModel(MNP entity)
        {
            return new MNPModel
            {
                Id = entity.Id,
                Number = entity.Number,
                Provider_Id = entity.Provider_Id,
                Region_Id = entity.Region_Id
            };
        }

        /// <summary>
        /// Возвращает список MNP-номеров
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/MNPs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MNPModel>>> GetMNPs()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает MNP-номер
        /// </summary>
        /// <param name="id">ID MNP-номера</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/MNPs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MNPModel>> GetMNP(long id)
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
        /// Обновляет MNP-номер
        /// </summary>
        /// <param name="id">ID MNP-номера</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/MNPs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMNP(long id, MNPModel model)
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
        /// Добавляет MNP-номер
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/MNPs
        [HttpPost]
        public async Task<ActionResult<MNPModel>> PostMNP(MNPCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetMNP), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет MNP-номер
        /// </summary>
        /// <param name="id">ID MNP-номера</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/MNPs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMNP(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            if (entity == null) return NotFound();

            var result = Check(Operation.Delete);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));

            return NoContent();
        }
    }
}
