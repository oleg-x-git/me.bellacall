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
    [SwaggerTag("<code style='color:seagreen'>Клиенты &rarr; Аккаунты &rarr; Роли</code>")]
    public class AspNetUserRolesController : ApiController<AspNetUserRole, AspNetUserRoleModel>
    {
        public AspNetUserRolesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override AspNetUserRole GetEntity(AspNetUserRoleModel model)
        {
            return new AspNetUserRole
            {
                Id = model.Id,
                UserId = model.User_Id,
                RoleId = model.Role_Id,
                Campaign_Id = model.Campaign_Id
            };
        }

        protected override AspNetUserRoleModel GetModel(AspNetUserRole entity)
        {
            return new AspNetUserRoleModel
            {
                Id = entity.Id,
                User_Id = entity.UserId,
                Role_Id = entity.RoleId,
                Campaign_Id = entity.Campaign_Id
            };
        }

        /// <summary>
        /// Возвращает список ролей аккаунта
        /// </summary>
        /// <param name="user_Id">USER_ID роли аккаунта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUserRoles/1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetUserRoleModel>>> GetAspNetUserRoles([FromQuery(Name = "oid")] long[] user_Id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(entity => user_Id.Contains(entity.UserId))
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает роль аккаунта
        /// </summary>
        /// <param name="id">ID роли аккаунта</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUserRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetUserRoleModel>> GetAspNetUserRole(long id)
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
        /// Добавляет роль аккаунта
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/AspNetUserRoles
        [HttpPost]
        public async Task<ActionResult<AspNetUserRoleModel>> PostAspNetUserRole(AspNetUserRoleCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetAspNetUserRole), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет роль аккаунта
        /// </summary>
        /// <param name="id">ID роли аккаунта</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/AspNetUserRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetUserRole(long id)
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
