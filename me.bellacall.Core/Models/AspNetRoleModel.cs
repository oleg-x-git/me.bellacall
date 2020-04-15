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
    /// Роль
    /// </summary>
    public class AspNetRoleModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Уровень доступа (минимально допустимый уровень доступа аккаунта для получения роли)
        /// </summary>
        [Log]
        public AspNetUserLevel PermissibleLevel { get; set; }
    }

    /// <summary>
    /// Роль / create
    /// </summary>
    public class AspNetRoleCreateModel: AspNetRoleModel
    {
        public override long Id { get => 0; }
    }
}
