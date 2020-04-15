using me.bellacall.Core.Locales;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    public class AspNetUserClaim : IdentityUserClaim<long> { }
    public class AspNetUserLogin : IdentityUserLogin<long> { }
    public class AspNetRoleClaim : IdentityRoleClaim<long> { }
    public class AspNetUserToken : IdentityUserToken<long> { }

    /// <summary>
    /// Аккаунт
    /// </summary>
    public class AspNetUser : IdentityUser<long>, IEntity
    {
        public override long Id { get => base.Id; set => base.Id = value; }
        public override string UserName { get => base.UserName; set => base.UserName = value; }
        public override string Email { get => base.Email; set => base.Email = value; }
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }

        public override bool TwoFactorEnabled { get => base.TwoFactorEnabled; set => base.TwoFactorEnabled = value; }
        public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }
        public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }
        public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }

        /// <summary>
        /// Клиент
        /// </summary>
        [ForeignKey("Company_Id")]
        public virtual Company Company { get; set; }
        public long Company_Id { get; set; }

        /// <summary>
        /// Уровень доступа
        /// </summary>
        public AspNetUserLevel Level { get; set; } = AspNetUserLevel.Company;

        /// <summary>
        /// Журнал операций
        /// </summary>
        [InverseProperty("User")]
        public virtual IList<AspNetUserLog> UserLogs { get; set; }

        /// <summary>
        /// Роли аккаунта
        /// </summary>
        [InverseProperty("User")]
        public virtual IList<AspNetUserRole> UserRoles { get; set; }
    }

    /// <summary>
    /// Роль
    /// </summary>
    public class AspNetRole : IdentityRole<long>, IEntity
    {
        public override long Id { get => base.Id; set => base.Id = value; }
        public override string Name { get => base.Name; set => base.Name = value; }
        public override string NormalizedName { get => base.NormalizedName; set => base.NormalizedName = value; }
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }

        /// <summary>
        /// Уровень доступа (минимально допустимый уровень доступа аккаунта для получения роли)
        /// </summary>
        public AspNetUserLevel PermissibleLevel { get; set; }

        /// <summary>
        /// Разрешения
        /// </summary>
        [InverseProperty("Role")]
        public virtual IList<AspNetRolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Роли аккаунта
        /// </summary>
        [InverseProperty("Role")]
        public virtual IList<AspNetUserRole> UserRoles { get; set; }
    }

    /// <summary>
    /// Уровень доступа
    /// </summary>
    public enum AspNetUserLevel : int
    {
        /// <summary>
        /// Клиент (ограничение по COMPANY_ID)
        /// </summary>
        Company = 1,

        /// <summary>
        /// Техподдержка
        /// </summary>
        Support = 2,

        /// <summary>
        /// Бэк-офис
        /// </summary>
        Root = 3,
        
        /// <summary>
        /// Разработчик
        /// </summary>
        Dev = 4
    }

    /// <summary>
    /// Роль аккаунта
    /// </summary>
    public class AspNetUserRole : IdentityUserRole<long>, IEntity
    {
        public long Id { get; set; }

        public override long UserId { get => base.UserId; set => base.UserId = value; }
        public override long RoleId { get => base.RoleId; set => base.RoleId = value; }

        /// <summary>
        /// Аккаунт
        /// </summary>
        [NotMapped]
        public virtual AspNetUser User { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [NotMapped]
        public virtual AspNetRole Role { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long? Campaign_Id { get; set; }
    }

    /// <summary>
    /// Разрешение роли
    /// </summary>
    public class AspNetRolePermission : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual AspNetRole Role { get; set; }
        public long RoleId { get; set; }

        /// <summary>
        /// Таблица
        /// </summary>
        [Required, StringLength(64)]
        public string TableName { get; set; }

        /// <summary>
        /// Разрешенная операция
        /// </summary>
        public Operation Operation { get; set; }
    }

    /// <summary>
    /// Журнал операций аккаунта
    /// </summary>
    public class AspNetUserLog : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Аккаунт
        /// </summary>
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }
        public long UserId { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long? Campaign_Id { get; set; }

        /// <summary>
        /// Дата/время
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Таблица
        /// </summary>
        [Required, StringLength(50)]
        public string TableName { get; set; }

        /// <summary>
        /// Операция
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        public Guid Operation_Id { get; set; }

        /// <summary>
        /// Идентификатор редактируемой строки
        /// </summary>
        public long? Data_Id { get; set; }

        /// <summary>
        /// Новые значяения редактируемой строки
        /// </summary>
        public string Data_Json { get; set; }
    }

    /// <summary>
    /// Операция
    /// </summary>
    //[DisplayName("Операция")]
    public enum Operation : int
    {
        /// <summary>
        /// Просмотр
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Operation_Read")]
        Read = 1,

        /// <summary>
        /// Добавление
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Operation_Create")]
        Create = 2,

        /// <summary>
        /// Изменение
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Operation_Update")]
        Update = 3,

        /// <summary>
        /// Удаление
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Operation_Delete")]
        Delete = 4,

        /// <summary>
        /// Выполнение
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "Operation_Execute")]
        Execute = 5
    }

    /// <summary>
    /// Атрибут таблицы с разрешениями
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class HasPermissionsAttribute : Attribute
    {
        Operation[] _operations;

        public HasPermissionsAttribute(params Operation[] operations) { _operations = operations; }

        /// <summary>
        /// Доступные операции
        /// </summary>
        public IEnumerable<Operation> Operations { get { return _operations; } }
    }
}

