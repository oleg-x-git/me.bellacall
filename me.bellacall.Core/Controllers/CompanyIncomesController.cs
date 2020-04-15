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
    [SwaggerTag("<code style='color:seagreen'>Клиенты &rarr; Начисление средств</code>")]
    public class CompanyIncomesController : ApiController<CompanyIncome, CompanyIncomeModel>
    {
        public CompanyIncomesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override CompanyIncome GetEntity(CompanyIncomeModel model)
        {
            return new CompanyIncome
            {
                Id = model.Id,
                Company_Id = model.Company_Id,
                TimeStamp = model.TimeStamp,
                Type = model.Type,
                Amount = model.Amount,
                Note = model.Note
            };
        }

        protected override CompanyIncomeModel GetModel(CompanyIncome entity)
        {
            return new CompanyIncomeModel
            {
                Id = entity.Id,
                Company_Id = entity.Company_Id,
                TimeStamp = entity.TimeStamp,
                Type = entity.Type,
                Amount = entity.Amount,
                Note = entity.Note
            };
        }

        /// <summary>
        /// Возвращает список начислений
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/CompanyIncomes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyIncomeModel>>> GetCompanyIncomes()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => e.Company_Id == COMPANY_ID)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает начисление
        /// </summary>
        /// <param name="id">ID начисления</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/CompanyIncomes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyIncomeModel>> GetCompanyIncome(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Where(e => e.Company_Id == COMPANY_ID)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет начисление
        /// </summary>
        /// <param name="id">ID начисления</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/CompanyIncomes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyIncome(long id, CompanyIncomeModel model)
        {
            var result = Check(model.Company_Id == COMPANY_ID, Forbidden).OkNull() ?? Check(id == model.Id, BadRequest).OkNull() ?? Check(Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет начисление
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/CompanyIncomes
        [HttpPost]
        public async Task<ActionResult<CompanyIncomeModel>> PostCompanyIncome(CompanyIncomeCreateModel model)
        {
            var result = Check(model.Company_Id == COMPANY_ID, Forbidden).OkNull() ?? Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetCompanyIncome), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет начисление
        /// </summary>
        /// <param name="id">ID начисления</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/CompanyIncomes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyIncome(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            if (entity == null) return NotFound();

            var result = Check(entity.Company_Id == COMPANY_ID, Forbidden).OkNull() ?? Check(Operation.Delete);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));

            return NoContent();
        }
    }
}
