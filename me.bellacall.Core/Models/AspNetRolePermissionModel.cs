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
    /// Разрешение роли
    /// </summary>
    public class AspNetRolePermissionModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        [Log]
        public long Role_Id { get; set; }

        /// <summary>
        /// Таблица
        /// </summary>
        [Log, Required, StringLength(64)]
        public string TableName { get; set; }

        /// <summary>
        /// Разрешенная операция
        /// </summary>
        [Log]
        public Operation Operation { get; set; }
    }

    /// <summary>
    /// Разрешение роли / create
    /// </summary>
    public class AspNetRolePermissionCreateModel : AspNetRolePermissionModel
    {
        public override long Id { get => 0; }
    }

    /// <summary>
    /// Таблица роли
    /// </summary>
    public class AspNetRoleTableModel
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отображаемое название
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Доступные операции
        /// </summary>
        public List<Operation> Operations { get; set; }
    }
}
