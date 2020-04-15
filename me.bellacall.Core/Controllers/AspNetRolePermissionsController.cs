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
    [SwaggerTag("<code style='color:gray'>Роли &rarr; Разрешения</code>")]
    public class AspNetRolePermissionsController : ApiController<AspNetRolePermission, AspNetRolePermissionModel>
    {
        public AspNetRolePermissionsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override AspNetRolePermission GetEntity(AspNetRolePermissionModel model)
        {
            return new AspNetRolePermission
            {
                Id = model.Id,
                RoleId = model.Role_Id,
                TableName = model.TableName,
                Operation = model.Operation
            };
        }

        protected override AspNetRolePermissionModel GetModel(AspNetRolePermission entity)
        {
            return new AspNetRolePermissionModel
            {
                Id = entity.Id,
                Role_Id = entity.RoleId,
                TableName = entity.TableName,
                Operation = entity.Operation
            };
        }

        /// <summary>
        /// Возвращает список таблиц
        /// </summary>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetRoleTables
        [HttpGet("~/api/AspNetRoleTables")]
        public ActionResult<IEnumerable<AspNetRoleTableModel>> GetAspNetRoleTables()
        {
            var result = Check(DB.Roles, Operation.Read);
            if (result.Fail()) return result;

            return AspNetDbExtensions.GetTableFeatures().
                Select(entity => new AspNetRoleTableModel
                {
                    Name = entity.Name,
                    DisplayName = entity.DisplayName,
                    Operations = new List<Operation>(entity.Operations)
                })
                .ToList();
        }

        /// <summary>
        /// Возвращает список разрешений
        /// </summary>
        /// <param name="role_Id">ROLE_ID разрешения</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetRolePermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetRolePermissionModel>>> GetAspNetRolePermissions([FromQuery(Name = "oid")] long[] role_Id)
        {
            var result = Check(DB.Roles, Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(entity => role_Id.Contains(entity.RoleId))
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает разрешение
        /// </summary>
        /// <param name="id">ID разрешения</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetRolePermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetRolePermissionModel>> GetAspNetRolePermission(long id)
        {
            var result = Check(DB.Roles, Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }

        /// <summary>
        /// Обновляет разрешение
        /// </summary>
        /// <param name="id">ID разрешения</param>
        /// <param name="model">Данные</param>
        /// <response code="400">Неверный запрос</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        /// <response code="410">Объект удален другим позователем</response>
        /// <response code="412">Объект изменен другим пользователем</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // PUT: api/AspNetRolePermissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAspNetRolePermission(long id, AspNetRolePermissionModel model)
        {
            var result = Check(id == model.Id, BadRequest).OkNull() ?? Check(DB.Roles, Operation.Update).OkNull() ?? CheckIfMatch(model.Id);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB.Entry(entity).State = EntityState.Modified;
            try { await DB.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!DB_TABLE.Any(e => e.Id == id)) return NotFound(); else throw; }

            Log(DB_TABLE.GetName(), Operation.Update, null, model);

            return NoContent();
        }

        /// <summary>
        /// Добавляет разрешение
        /// </summary>
        /// <param name="model">Данные</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // POST: api/AspNetRolePermissions
        [HttpPost]
        public async Task<ActionResult<AspNetRolePermissionModel>> PostAspNetRolePermission(AspNetRolePermissionCreateModel model)
        {
            var result = Check(DB.Roles, Operation.Update);
            if (result.Fail()) return result;

            var entity = GetEntity(model);

            DB_TABLE.Add(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Create, null, GetModel(entity));

            return CreatedAtAction(nameof(GetAspNetRolePermission), new { id = entity.Id }, GetModel(entity));
        }

        /// <summary>
        /// Удаляет разрешение
        /// </summary>
        /// <param name="id">ID разрешения</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        // DELETE: api/AspNetRolePermissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAspNetRolePermission(long id)
        {
            var entity = await DB_TABLE.FindAsync(id);
            if (entity == null) return NotFound();

            var result = Check(DB.Roles, Operation.Update);
            if (result.Fail()) return result;

            DB_TABLE.Remove(entity);
            await DB.SaveChangesAsync();

            Log(DB_TABLE.GetName(), Operation.Delete, null, GetModel(entity));

            return NoContent();
        }
    }
}
