using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using me.bellacall.Core.Locales;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace me.bellacall.Core.Data
{
    using o = Operation;

    public interface IEntity { long Id { get; set; } }

    public interface IModel { long Id { get; set; } }

    public class AspNetDbContext : IdentityDbContext<AspNetUser, AspNetRole, long, AspNetUserClaim, AspNetUserRole, AspNetUserLogin, AspNetRoleClaim, AspNetUserToken>
    {
        #region .. ctor ..

        public AspNetDbContext(DbContextOptions<AspNetDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AspNetUserRole>(entity =>
            {
                var key = entity.HasKey(e => new { e.UserId, e.RoleId }).Metadata;
                entity.Metadata.RemoveKey(key.Properties);
            });
            builder.Entity<AspNetUserRole>().HasKey(e => new { e.Id });
            builder.Entity<AspNetRolePermission>().ToTable("AspNetRolePermissions");
            builder.Entity<AspNetUserLog>().ToTable("AspNetUserLogs");

            // индексы
            builder.Entity<AspNetUserLog>().HasIndex(e => new { e.TimeStamp });

            // запрещаем каскадное удаление для всех
            foreach (var fk in builder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)) fk.DeleteBehavior = DeleteBehavior.Restrict;

            //builder.Entity<InboxScript>().HasOne(e => e.Script).WithMany().OnDelete(DeleteBehavior.Restrict);
            //builder.Entity<JobScript>().HasOne(e => e.Script).WithMany().OnDelete(DeleteBehavior.Restrict);
            //builder.Entity<JobContact>().HasOne(e => e.Contact).WithMany().OnDelete(DeleteBehavior.Restrict);
            //builder.Entity<LeadContact>().HasOne(e => e.Contact).WithMany().OnDelete(DeleteBehavior.Restrict);
        }

        #endregion

        #region .. Common ..

        /// <summary>
        /// Справочники - DEF-коды
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_DEFs"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Common.DEF> DEFs { get; set; }

        /// <summary>
        /// Справочники - Перенесенные номера
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_MNPs"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Common.MNP> MNPs { get; set; }

        /// <summary>
        /// Справочники - Операторы связи
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Providers"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Common.Provider> Providers { get; set; }

        /// <summary>
        /// Справочники - Регионы
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Regions"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Common.Region> Regions { get; set; }

        /// <summary>
        /// Справочники - Тарифы
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Tariffs"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Common.Tariff> Tariffs { get; set; }

        /// <summary>
        /// Справочники - Тарифы - Условия
        /// </summary>
        public DbSet<Common.TariffCondition> TariffConditions { get; set; }

        #endregion

        #region .. Identity ..

        /// <summary>
        /// Справочники - Роли
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Roles"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public new DbSet<AspNetRole> Roles { get; set; }

        /// <summary>
        /// Справочники - Роли - Разрешения
        /// </summary>
        public DbSet<AspNetRolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Клиенты - Аккаунты
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Users"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public new DbSet<AspNetUser> Users { get; set; }

        /// <summary>
        /// Клиенты - Аккаунты - Роли
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_UserRoles"), HasPermissions(o.Read, o.Create, o.Delete)]
        public new DbSet<AspNetUserRole> UserRoles { get; set; }

        /// <summary>
        /// Клиенты - Аккаунты - Журнал операций
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_UserLogs"), HasPermissions(o.Read)]
        public DbSet<AspNetUserLog> UserLogs { get; set; }

        public new DbSet<AspNetUserClaim> UserClaims { get; set; }
        public new DbSet<AspNetUserLogin> UserLogins { get; set; }
        public new DbSet<AspNetUserToken> UserTokens { get; set; }
        public new DbSet<AspNetRoleClaim> RoleClaims { get; set; }

        #endregion

        #region .. Companies ..

        /// <summary>
        /// Клиенты
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Companies"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Company> Companies { get; set; }

        /// <summary>
        /// Клиенты - Списания средств
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_CompanyExpenses"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<CompanyExpense> CompanyExpenses { get; set; }

        /// <summary>
        /// Клиенты - Начисления средств
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_CompanyIncomes"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<CompanyIncome> CompanyIncomes { get; set; }

        #endregion

        #region .. Campaigns ..

        /// <summary>
        /// Кампании
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Campaigns"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Кампании - Свойства лида
        /// </summary>
        public DbSet<LeadProperty> LeadProperties { get; set; }

        /// <summary>
        /// Кампании - Свойства контакта
        /// </summary>
        public DbSet<ContactProperty> ContactProperties { get; set; }

        #endregion

        #region .. ContactGroups ..

        /// <summary>
        /// Кампании - Группы контактов
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_ContactGroups"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<ContactGroup> ContactGroups { get; set; }

        /// <summary>
        /// Кампании - Группы контактов - Контакты
        /// </summary>
        public DbSet<Contact> Contacts { get; set; }

        #endregion

        #region .. Gateways ..

        /// <summary>
        /// Кампании - Шлюзы
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Gateways"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Gateway> Gateways { get; set; }

        /// <summary>
        /// Кампании - Шлюзы - Потоки
        /// </summary>
        public DbSet<GatewayStream> GatewayStreams { get; set; }

        #endregion

        #region .. Jobs ..

        /// <summary>
        /// Кампании - Рассылки
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Jobs"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Job> Jobs { get; set; }

        /// <summary>
        /// Кампании - Рассылки - Контакты
        /// </summary>
        public DbSet<JobContact> JobContacts { get; set; }

        /// <summary>
        /// Кампании - Рассылки - Сценарии рассылки
        /// </summary>
        public DbSet<JobScript> JobScripts { get; set; }

        /// <summary>
        /// Кампании - Рассылки - Сценарии коллбэка
        /// </summary>
        public DbSet<InboxScript> InboxScripts { get; set; }

        #endregion

        #region .. Leads ..

        /// <summary>
        /// Кампании - Лиды
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Leads"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Lead> Leads { get; set; }

        /// <summary>
        /// Кампании - Лиды - Контакты
        /// </summary>
        public DbSet<LeadContact> LeadContacts { get; set; }

        #endregion

        #region .. Scripts ..

        /// <summary>
        /// Кампании - Сценарии
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "DbContext_Scripts"), HasPermissions(o.Read, o.Create, o.Update, o.Delete)]
        public DbSet<Script> Scripts { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Элементы
        /// </summary>
        public DbSet<ScriptElement> ScriptElements { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Элементы - Условные переходы
        /// </summary>
        public DbSet<ScriptCondition> ScriptConditions { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Элементы - Входные параметры
        /// </summary>
        public DbSet<ScriptInputParameter> ScriptInputParameters { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Элементы - Выходные параметры
        /// </summary>
        public DbSet<ScriptOutputParameter> ScriptOutputParameters { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Переменные
        /// </summary>
        public DbSet<ScriptVariable> ScriptVariables { get; set; }

        /// <summary>
        /// Кампании - Сценарии - Свойства (статические переменные)
        /// </summary>
        public DbSet<ScriptProperty> ScriptProperties { get; set; }

        #endregion
    }
}
