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
    /// Клиент
    /// </summary>
    public class CompanyModel : IModel
    {
        public long Id { get; set; }

        /// <summary>
        /// ФИО / Название
        /// </summary>
        [Log, Required, StringLength(256)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Клиент / create
    /// </summary>
    public class CompanyCreateModel : AuthModel
    {
        /// <summary>
        /// ФИО / Название
        /// </summary>
        [Log, Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public override string Email { get => base.Email; set => base.Email = value; }

        /// <summary>
        /// Пароль
        /// </summary>
        public override string Password { get => base.Password; set => base.Password = value; }
    }
}
