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
    [SwaggerTag("<code style='color:gray'>Роли</code>")]
    public class AspNetRolesController : ApiController<AspNetRole, AspNetRoleModel>
    {
        public AspNetRolesController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override AspNetRole GetEntity(AspNetRoleModel model)
        {
            return new AspNetRole
            {
                Id = model.Id,
                Name = model.Name,
                PermissibleLevel = model.PermissibleLevel
            };
        }

        protected override AspNetRoleModel GetModel(AspNetRole entity)
        {
            return new AspNetRoleModel
            {
                Id = entity.Id,
                Name = entity.Name,
                PermissibleLevel = entity.PermissibleLevel
            };
        }

        /// <summary>
        /// Возвращает список ролей
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetRoleModel>>> GetAspNetRoles()
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает роль
        /// </summary>
        /// <param name="id">ID роли</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetRoleModel>> GetAspNetRole(long id)
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
        /// Обновляет роль
        /// </summary>
        /// <param name="id">ID роли</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/AspNetRoles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetRole(long id, AspNetRoleModel model)
        {
            var result = Check(id == model.Id, BadRequest).OkNull() ?? Check(Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = await DB_TABLE.FirstOrDefaultAsync(e => e.Id == model.Id);
            entity.Name = model.Name;
            entity.NormalizedName = model.Name.ToUpper();
            entity.ConcurrencyStamp = Guid.NewGuid().ToString();
            entity.PermissibleLevel = model.PermissibleLevel;

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет роль
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/AspNetRoles
        [HttpPost]
        public async Task<ActionResult<AspNetRoleModel>> PostAspNetRole(AspNetRoleCreateModel model)
        {
            var result = Check(Operation.Create);
            if (result.Fail()) return result;

            var entity = new AspNetRole
            {
                Name = model.Name,
                NormalizedName = model.Name.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PermissibleLevel = model.PermissibleLevel
            };

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetAspNetRole), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет роль
        /// </summary>
        /// <param name="id">ID роли</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/AspNetRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetRole(long id)
        {
            var entity = await DB_TABLE.Include(e => e.RolePermissions).Include(e => e.UserRoles).FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return NotFound();

            var result = Check(Operation.Delete);
            if (result.Fail()) return result;

            {
                DB.RemoveRange(entity.RolePermissions);
                DB.RemoveRange(entity.UserRoles);
                DB_TABLE.Remove(entity);
            }

            await DB.SaveChangesAsync();

            var operation_Id = Guid.NewGuid();
            {
                Log(DB_TABLE.GetName(), Operation.Delete, operation_Id, null, GetModel(entity));
                Get<AspNetRolePermissionsController>().Log(Operation.Delete, operation_Id, null, entity.RolePermissions);
                Get<AspNetUserRolesController>().Log(Operation.Delete, operation_Id, null, entity.UserRoles);
            }

            return NoContent();
        }
    }
}
