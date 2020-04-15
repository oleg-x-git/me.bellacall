using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Аккаунт
    /// </summary>
    public class AspNetUserModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [Log]
        public long Company_Id { get; set; }

        /// <summary>
        /// Имя пользователя аккаунта
        /// </summary>
        [Log, StringLength(256)]
        public string UserName { get; set; }

        /// <summary>
        /// Почтовый ящик
        /// </summary>
        [Log, Required, StringLength(256), EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Log, StringLength(256), Phone]
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// Аккаунт / root
    /// </summary>
    public class AspNetUserRootModel : AspNetUserModel
    {
        /// <summary>
        /// Уровень доступа
        /// </summary>
        public AspNetUserLevel Level { get; set; }

        /// <summary>
        /// Почтовый ящик подтвержден
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Номер телефона подтвержден
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Двухфакторная аутентификация включена
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Блокировка включена
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Дата/время завершения блокировки
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Количество неудачных попыток входя в систему
        /// </summary>
        public int AccessFailedCount { get; set; }
    }

    /// <summary>
    /// Аккаунт / create
    /// </summary>
    public class AspNetUserCreateModel : AspNetUserModel
    {
        public override long Id { get => 0; }

        [Required, StringLength(32)]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }
    }
}
