using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using me.bellacall.Core.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using me.bellacall.Core.Locales;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace me.bellacall.Core.Controllers
{
    [SwaggerTag("<code style='color:seagreen'>Клиенты</code>")]
    public class CompaniesController : ApiController<Company, CompanyModel>
    {
        public CompaniesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override Company GetEntity(CompanyModel model)
        {
            return new Company
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        protected override CompanyModel GetModel(Company entity)
        {
            return new CompanyModel
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Возвращает список клиентов
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyModel>>> GetCompanies()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(e => e.Id == COMPANY_ID)
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyModel>> GetCompany(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .Where(e => e.Id == COMPANY_ID)
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(long id, CompanyModel model)
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
        /// Добавляет клиента
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/Companies
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CompanyModel>> PostCompany(CompanyCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = new Company
            {
                Name = model.Name
            };

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            var user = new AspNetUser { UserName = model.Name, Email = model.Email, Company_Id = entity.Id, Level = AspNetUserLevel.Company };
            var identityResult = await UserManager.CreateAsync(user, model.Password);

            if (identityResult.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false);
                if (User.Identity is ClaimsIdentity)
                {
                    ((ClaimsIdentity)User.Identity).AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.Integer64));

                    Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));
                    Log(DB.Users.GetName(), Operation.Create, null, model);
                }

                return CreatedAtAction(nameof(GetCompany), new { id = entity.Id }, GetModel(entity));
            }

            if ((entity = await DB_TABLE.FindAsync(entity.Id)) != null)
            {
                DB_TABLE.Remove(entity);
                await DB.SaveChangesAsync();
            }

            return BadRequest(string.Join(";", identityResult.Errors.Select(e => e.Description)));
        }

        /// <summary>
        /// Удаляет клиента
        /// </summary>
        /// <param name="id">ID клиента</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(long id)
        {
            var entity = await DB_TABLE
                .Include(e => e.Campaigns)      // stop
                .Include(e => e.CompanyExpenses)
                .Include(e => e.CompanyIncomes)
                .Include(e => e.Users)
                .ThenInclude(e => e.UserLogs)
                .Include(e => e.Users)
                .ThenInclude(e => e.UserRoles)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entity == null) return NotFound();

            var result = Check(Operation.Delete).OkNull() ??
                Check(entity.Campaigns.Count == 0, Forbidden, string.Format(Strings.Cascade_Message, Strings.Company_Entity, Strings.Campaign_List));

            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.Users.SelectMany(e => e.UserLogs));
                DB.RemoveRange(entity.Users.SelectMany(e => e.UserRoles));
                DB.RemoveRange(entity.Users);
                DB.RemoveRange(entity.CompanyExpenses);
                DB.RemoveRange(entity.CompanyIncomes);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));
                Get<AspNetUserLogsController>().Log(Operation.Delete, operation_Id, null, entity.Users.SelectMany(e => e.UserLogs));
                Get<AspNetUserRolesController>().Log(Operation.Delete, operation_Id, null, entity.Users.SelectMany(e => e.UserRoles));
                Get<AspNetUsersController>().Log(Operation.Delete, operation_Id, null, entity.Users);
                Get<CompanyExpensesController>().Log(Operation.Delete, operation_Id, null, entity.CompanyExpenses);
                Get<CompanyIncomesController>().Log(Operation.Delete, operation_Id, null, entity.CompanyIncomes);
            }

            return NoContent();
        }
    }
}
