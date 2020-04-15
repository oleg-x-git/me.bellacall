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
    [SwaggerTag("<code style='color:seagreen'>Клиенты &rarr; Аккаунты</code>")]
    public class AspNetUsersController : ApiController<AspNetUser, AspNetUserModel>
    {
        public AspNetUsersController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override AspNetUser GetEntity(AspNetUserModel model)
        {
            return new AspNetUser
            {
                Id = model.Id,
                Company_Id = model.Company_Id,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
        }

        protected override AspNetUserModel GetModel(AspNetUser entity)
        {
            return new AspNetUserModel
            {
                Id = entity.Id,
                Company_Id = entity.Company_Id,
                UserName = entity.UserName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber
            };
        }

        /// <summary>
        /// Возвращает список аккаунтов
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetUserModel>>> GetAspNetUsers()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает аккаунт
        /// </summary>
        /// <param name="id">ID аккаунта</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetUserModel>> GetAspNetUser(long id)
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
        /// Обновляет аккаунт
        /// </summary>
        /// <param name="id">ID аккаунта</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/AspNetUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetUser(long id, AspNetUserModel model)
        {
            var result = Check(id == model.Id, BadRequest).OkNull() ?? Check(Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = await DB_TABLE.FirstOrDefaultAsync(e => e.Id == model.Id);
            entity.Company_Id = model.Company_Id;
            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.PhoneNumber = model.PhoneNumber;
            var identityResult = await UserManager.UpdateAsync(entity);

            if (!identityResult.Succeeded) return BadRequest(string.Join(";", identityResult.Errors.Select(e => e.Description)));

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет аккаунт
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/AspNetUsers
        [HttpPost]
        public async Task<ActionResult<AspNetUserModel>> PostAspNetUser(AspNetUserCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);
            var identityResult = await UserManager.CreateAsync(entity, model.Password);

            if (!identityResult.Succeeded) return BadRequest(string.Join(";", identityResult.Errors.Select(e => e.Description)));

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetAspNetUser), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет аккаунт
        /// </summary>
        /// <param name="id">ID аккаунта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/AspNetUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetUser(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            if (entity == null) return NotFound();

            var result = Check(Operation.Delete);
            if (result.Fail()) return result;

            var identityResult = await UserManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
            if (identityResult.Succeeded) identityResult = await UserManager.UpdateSecurityStampAsync(entity);

            if (identityResult.Succeeded)
            {
                Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));
                return NoContent();
            }

            return BadRequest(string.Join(";", identityResult.Errors.Select(e => e.Description)));
        }
    }
}
