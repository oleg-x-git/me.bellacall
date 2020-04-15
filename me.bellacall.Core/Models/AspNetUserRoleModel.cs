using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Роль аккаунта
    /// </summary>
    public class AspNetUserRoleModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Аккаунт
        /// </summary>
        [Log]
        public long User_Id { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [Log]
        public long Role_Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long? Campaign_Id { get; set; }
    }

    /// <summary>
    /// Роль аккаунта / create
    /// </summary>
    public class AspNetUserRoleCreateModel : AspNetUserRoleModel
    {
        public override long Id { get => 0; }
    }
}
