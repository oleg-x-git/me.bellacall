using me.bellacall.Core.Data;
using me.bellacall.Core.Locales;
using me.bellacall.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace me.bellacall.Core.Controllers
{
#if !Debug
    [RequireHttps]
#endif
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected const string ETAG_HEADER = "ETag";
        protected const string MATCH_HEADER = "If-Match";
        protected const bool ETag304 = false;

        public ApiController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger)
        {
            DB = context;
            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
            Logger = logger;
        }

        protected UserManager<AspNetUser> UserManager { get; private set; }
        protected SignInManager<AspNetUser> SignInManager { get; private set; }
        protected IEmailSender EmailSender { get; private set; }
        protected ILogger<AuthController> Logger { get; private set; }

        protected AspNetDbContext DB { get; }

        /// <summary>
        /// Возвращает 403 Forbidden с сообщением
        /// </summary>
        [NonAction]
        public virtual ObjectResult Forbidden(object value)
        {
            return StatusCode(StatusCodes.Status403Forbidden, value);
        }

        /// <summary>
        /// Возвращает 403 Forbidden
        /// </summary>
        [NonAction]
        public virtual StatusCodeResult Forbidden()
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Идентификатор клиента аутентифицированного аккаунта
        /// </summary>
        protected long COMPANY_ID { get => long.Parse(User.FindFirst(AuthOptions.Company).Value); }

        /// <summary>
        /// Уровень доступа аутентифицированного аккаунта
        /// </summary>
        protected AspNetUserLevel LEVEL { get => (AspNetUserLevel)Enum.Parse(typeof(AspNetUserLevel), User.FindFirst(AuthOptions.Level).Value); }

        /// <summary>
        /// Возвращает контроллер заданного типа
        /// </summary>
        /// <typeparam name="TController">Контроллер</typeparam>
        protected TController Get<TController>() where TController : ApiController
        {
            var controller = (TController)typeof(TController).GetConstructors()[0].Invoke(new object[] { DB, UserManager, SignInManager, EmailSender, Logger });
            controller.ControllerContext = ControllerContext;
            return controller;
        }
    }

    public abstract class ApiController<TEntity> : ApiController where TEntity : class, IEntity, new()
    {
        public ApiController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        /// <summary>
        /// Таблица
        /// </summary>
        protected DbSet<TEntity> DB_TABLE { get => DB.GetTable<TEntity>(); }

        protected string GetForbiddenMessage<T>(DbSet<T> table, Operation operation) where T : class
        {
            return string.Format(
                Strings.Permission_Message,
                Enum.GetName(typeof(Operation), operation),
                AspNetDbExtensions.GetTableFeature(table.GetName()).DisplayName);
        }

        #region .. Check ..

        public delegate ActionResult StatusCodeResultDelegate();
        public delegate ActionResult ObjectResultDelegate(object value);

        protected ActionResult Check(bool condition, StatusCodeResultDelegate result)
        {
            return condition ? Ok() : result();
        }

        protected ActionResult Check(bool condition, ObjectResultDelegate result, object value)
        {
            return condition ? Ok() : result(value);
        }

        /// <summary>
        /// Проверяет допустимость операции
        /// </summary>
        /// <typeparam name="T">Тип сущности проверяемой таблицы</typeparam>
        /// <param name="table">Таблица</param>
        /// <param name="operation">Операция</param>
        /// <param name="сampaign_Id">Идентификатор кампании</param>
        protected ActionResult Check<T>(DbSet<T> table, Operation operation, long сampaign_Id) where T : class
        {
#if NoUsers
            return Ok();
#endif

            var tableName = table.GetName();
            var user_Id = User.Identity.Id();

            var userRoles = DB.UserRoles
                .Where(e => (!e.Campaign_Id.HasValue || e.Campaign_Id.Value == сampaign_Id) && e.UserId == user_Id)
                .Where(e => e.Role.PermissibleLevel <= LEVEL)
                .Select(e => e.RoleId);

            var allow = DB.RolePermissions
                .Where(e => e.TableName == tableName && e.Operation == operation)
                .Join(userRoles, o => o.RoleId, i => i, (o, i) => o)
                .Any();

            return allow ? Ok() as ActionResult : Forbidden(GetForbiddenMessage(table, operation));
        }

        /// <summary>
        /// Проверяет допустимость операции (для таблицы этого контроллера)
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="сampaign_Id">Идентификатор кампании</param>
        protected ActionResult Check(Operation operation, long сampaign_Id)
        {
            return Check(DB_TABLE, operation, сampaign_Id);
        }

        #endregion

        #region .. Check без кампании ..

        /// <summary>
        /// Проверяет допустимость операции без проверки кампании
        /// </summary>
        /// <typeparam name="T">Тип сущности проверяемой таблицы</typeparam>
        /// <param name="table">Таблица</param>
        /// <param name="operation">Операция</param>
        protected ActionResult Check<T>(DbSet<T> table, Operation operation) where T : class
        {
#if NoUsers
            return Ok();
#endif

            var tableName = table.GetName();
            var user_Id = User.Identity.Id();

            var userRoles = DB.UserRoles
                .Where(e => e.UserId == user_Id)
                .Where(e => e.Role.PermissibleLevel <= LEVEL)
                .Select(e => e.RoleId)
                .Distinct();

            var allow = DB.RolePermissions
                .Where(e => e.TableName == tableName && e.Operation == operation)
                .Join(userRoles, o => o.RoleId, i => i, (o, i) => o)
                .Any();

             return allow ? Ok() as ActionResult : Forbidden(GetForbiddenMessage(table, operation));
        }

        /// <summary>
        /// Проверяет допустимость операции (для таблицы этого контроллера) без проверки кампании
        /// </summary>
        /// <param name="operation">Операция</param>
        protected ActionResult Check(Operation operation)
        {
            return Check(DB_TABLE, operation);
        }

        #endregion

        #region .. Allowed Ids ..

        /// <summary>
        /// Список идентификаторов кампаний, для которых разрешена заданная операция
        /// </summary>
        /// <typeparam name="T">Тип сущности проверяемой таблицы</typeparam>
        /// <param name="table">Таблица</param>
        /// <param name="operation">Операция</param>
        protected IQueryable<long> AllowedIds<T>(DbSet<T> table, Operation operation) where T : class
        {
#if NoUsers
            return DB.Campaigns.Select(e => e.Id);
#endif

            var tableName = table.GetName();
            var user_Id = User.Identity.Id();

            var roles = DB.RolePermissions
                .Where(e => e.TableName == tableName && e.Operation == operation)
                .Where(e => e.Role.PermissibleLevel <= LEVEL)
                .Select(e => e.RoleId);

            // все роли пользователя, для которых разрешена заданныя операция
            var allowedRoles = DB.UserRoles
                .Where(e => e.UserId == user_Id)
                .Select(e => new { RoleId = e.RoleId, Campaign_Id = e.Campaign_Id })
                .Join(roles, o => o.RoleId, i => i, (o, i) => o);

            if (allowedRoles.Any(e => !e.Campaign_Id.HasValue)) return DB.Campaigns.Where(e => e.Company_Id == COMPANY_ID).Select(e => e.Id);   // одна из ролей без ограничений - все кампании
            else return allowedRoles.Select(e => e.Campaign_Id.Value).Distinct();  // только назначеннные кампании
        }

        /// <summary>
        /// Список идентификаторов кампаний (для таблицы этого контроллера), для которых разрешена заданная операция
        /// </summary>
        /// <param name="operation">Операция</param>
        protected IQueryable<long> AllowedIds(Operation operation)
        {
            return AllowedIds(DB_TABLE, operation);
        }

        #endregion

        #region .. ETag / If-Match ..

        protected void HashToETag(string hash)
        {
            HttpContext.Response.Headers.Add(ETAG_HEADER, hash);
        }

        protected StatusCodeResult CheckETag(string hash)
        {
            HashToETag(hash);
            return (ETag304 && HttpContext.Request.Headers.ContainsKey(MATCH_HEADER) && HttpContext.Request.Headers[MATCH_HEADER].RemoveQuotes() == hash) ? StatusCode(StatusCodes.Status304NotModified) : new OkResult();
        }

        protected StatusCodeResult CheckIfMatch(string hash)
        {
            HashToETag(hash);
            return !HttpContext.Request.Headers.ContainsKey(MATCH_HEADER) || HttpContext.Request.Headers[MATCH_HEADER].RemoveQuotes() != hash ? StatusCode(StatusCodes.Status412PreconditionFailed) : new OkResult();
        }

        protected StatusCodeResult CheckIfMatch<T>(DbSet<T> table, long id) where T : class, IEntity
        {
            var hash = table.AsNoTracking().FirstOrDefault(e => e.Id == id)?.GetHash();
            return hash == null ? StatusCode(StatusCodes.Status410Gone) : CheckIfMatch(hash);
        }

        protected StatusCodeResult CheckIfMatch(long id)
        {
            return CheckIfMatch(DB_TABLE, id);
        }

        #endregion

        #region .. Log ..

        protected string ValueToString(object value)
        {
            if (value == null) return "-";
            if (value is bool) return (bool)value ? Strings.Common_Yes : Strings.Common_No;

            var type = value.GetType();

            if (type.IsEnum)
            {
                var member = type.GetMember(value.ToString()).FirstOrDefault();
                return member == null ? "-" : member.CustomAttributes.Any(a => a.AttributeType == typeof(DisplayAttribute)) ? member.GetCustomAttribute<DisplayAttribute>().ResourceType != typeof(Strings) ? member.GetCustomAttribute<DisplayAttribute>(false).Name : Strings.ResourceManager.GetString(member.GetCustomAttribute<DisplayAttribute>(false).Name) : value.ToString();
            }

            return value.ToString();
        }

        /// <summary>
        /// Логирует действие аккаунта
        /// </summary>
        /// <typeparam name="TModel">Тип модели</typeparam>
        /// <param name="tableName">Таблица</param>
        /// <param name="operation">Операция</param>
        /// <param name="operation_id">Идентификатор операции</param>
        /// <param name="campaign_Id">Идентификатор кампании</param>
        /// <param name="models">Данные</param>
        protected void Log<TModel>(string tableName, Operation operation, Guid operation_id, long? campaign_Id, params TModel[] models) where TModel : class
        {
            var user_Id = User.Identity.Id();

            foreach (var data in models.Select(model => new
            {
                id = (model as IModel)?.Id,
                json = JsonConvert.SerializeObject(typeof(TModel)
                     .GetProperties()
                     .Where(property => property.CustomAttributes.Any(a => a.AttributeType == typeof(LogAttribute)))
                     .Select(p => new { key = p.Name, value = ValueToString(p.GetValue(model)) })
                     .ToDictionary(e => e.key, e => e.value))
            })) DB.UserLogs.Add(new AspNetUserLog
            {
                TimeStamp = DateTime.Now,
                TableName = tableName,
                Campaign_Id = campaign_Id,
                Operation = operation,
                Operation_Id = operation_id,
                UserId = user_Id.Value,
                Data_Id = data.id,
                Data_Json = data.json
            });

            DB.SaveChanges();
        }

        /// <summary>
        /// Логирует действие аккаунта
        /// </summary>
        /// <typeparam name="TModel">Тип модели</typeparam>
        /// <param name="tableName">Таблица</param>
        /// <param name="operation">Операция</param>
        /// <param name="campaign_Id">Идентификатор кампании</param>
        /// <param name="models">Данные</param>
        protected void Log<TModel>(string tableName, Operation operation, long? campaign_Id, params TModel[] models) where TModel : class { Log(tableName, operation, Guid.NewGuid(), campaign_Id, models); }

        #endregion
    }

    public static class ApiControllerHelpers
    {
        public static long? Id(this IIdentity identity)
        {
            var id = identity is ClaimsIdentity ? ((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
            return string.IsNullOrEmpty(id) ? null : new long?(long.Parse(id));
        }

        /// <summary>
        /// Возвращает null для Ok-результатов (2xx) или проверяемый результат
        /// </summary>
        /// <param name="result">Проверяемый результат</param>
        public static ActionResult OkNull(this ActionResult result)
        {
            return result is OkResult || result is OkObjectResult || result is NoContentResult ? null : result;
        }

        /// <summary>
        /// Возвращает true для non-Ok-результатов или false
        /// </summary>
        /// <param name="result">Проверяемый результат</param>
        public static bool Fail(this ActionResult result)
        {
            return !(result is OkResult || result is OkObjectResult || result is NoContentResult);
        }

        public static ModelStateDictionary AddError(this ModelStateDictionary modelState, string code, string description)
        {
            modelState.TryAddModelError(code, description);
            return modelState;
        }

        public static string RemoveQuotes(this StringValues value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            var s = value.ToString().Replace("\"", "");
            return value.ToString().Replace("\"", "");
        }
    }

    public abstract class ApiController<TEntity, TModel> : ApiController<TEntity> where TEntity : class, IEntity, new() where TModel : class, new()
    {
        public ApiController(AspNetDbContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IEmailSender emailSender, ILogger<AuthController> logger) : base(context, userManager, signInManager, emailSender, logger) { }

        protected virtual TModel GetModel(TEntity entity)
        {
            return new TModel();
        }

        protected virtual TEntity GetEntity(TModel model)
        {
            return new TEntity();
        }

        /// <summary>
        /// Логирует действие аккаунта
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="operation_id">Идентификатор операции</param>
        /// <param name="campaign_Id">Идентификатор кампании</param>
        /// <param name="entities">Данные (IEntity)</param>
        [NonAction]
        public void Log(Operation operation, Guid operation_id, long? campaign_Id, params TEntity[] entities) { Log(operation, operation_id, campaign_Id, entities); }

        /// <summary>
        /// Логирует действие аккаунта
        /// </summary>
        /// <param name="operation">Операция</param>
        /// <param name="operation_id">Идентификатор операции</param>
        /// <param name="campaign_Id">Идентификатор кампании</param>
        /// <param name="entities">Данные (IEntity)</param>
        [NonAction]
        public void Log(Operation operation, Guid operation_id, long? campaign_Id, IEnumerable<TEntity> entities)
        {
            Log(DB_TABLE.GetName(), operation, operation_id, campaign_Id, entities.Select(entity => GetModel(entity)).ToArray());
        }
    }

    /// <summary>
    /// Устанавливает необходимость включения свойства в AspNetUserLogs
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LogAttribute : Attribute { }
}
