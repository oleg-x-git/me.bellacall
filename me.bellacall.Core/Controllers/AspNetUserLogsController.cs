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
    [SwaggerTag("<code style='color:seagreen'>Клиенты &rarr; Аккаунты &rarr; Журнал операций</code>")]
    public class AspNetUserLogsController : ApiController<AspNetUserLog, AspNetUserLogModel>
    {
        public AspNetUserLogsController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected override AspNetUserLog GetEntity(AspNetUserLogModel model)
        {
            return new AspNetUserLog
            {
                Id = model.Id,
                UserId = model.User_Id,
                Campaign_Id = model.Campaign_Id,
                TimeStamp = model.TimeStamp,
                TableName = model.TableName,
                Operation = model.Operation,
                Operation_Id = model.Operation_Id,
                Data_Id = model.Data_Id,
                Data_Json = model.Data_Json
            };
        }

        protected override AspNetUserLogModel GetModel(AspNetUserLog entity)
        {
            return new AspNetUserLogModel
            {
                Id = entity.Id,
                User_Id = entity.UserId,
                Campaign_Id = entity.Campaign_Id,
                TimeStamp = entity.TimeStamp,
                TableName = entity.TableName,
                Operation = entity.Operation,
                Operation_Id = entity.Operation_Id,
                Data_Id = entity.Data_Id,
                Data_Json = entity.Data_Json
            };
        }

        /// <summary>
        /// Возвращает журнал операций
        /// </summary>
        /// <param name="user_Id">USER_ID записей</param>
        /// <response code="403">Нет прав на выполнение операции</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUserLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AspNetUserLogModel>>> GetAspNetUserLogs([FromQuery(Name = "oid")] long[] user_Id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            return await DB_TABLE
                .Where(entity => user_Id.Contains(entity.UserId))
                .Select(entity => GetModel(entity))
                .ToListAsync();
        }

        /// <summary>
        /// Возвращает запись из журнала операций
        /// </summary>
        /// <param name="id">ID записи</param>
        /// <response code="304">Объект не модифицирован</response>
        /// <response code="403">Нет прав на выполнение операции</response>
        /// <response code="404">Объект не найден</response>
        [SwaggerResponse(StatusCodes.Status200OK)]
        // GET: api/AspNetUserLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AspNetUserLogModel>> GetAspNetUserLog(long id)
        {
            var result = Check(Operation.Read);
            if (result.Fail()) return result;

            var entity = await DB_TABLE
                .FirstOrDefaultAsync(e => e.Id == id);

            result = Check(entity != null, NotFound).OkNull() ?? CheckETag(entity.GetHash());
            if (result.Fail()) return result;

            return GetModel(entity);
        }
    }
}
