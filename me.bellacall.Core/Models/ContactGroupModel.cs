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
    /// Группа контактов
    /// </summary>
    public class ContactGroupModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [Log, StringLength(256)]
        public string Description { get; set; }
    }

    /// <summary>
    /// Группа контактов / create
    /// </summary>
    public class ContactGroupCreateModel : ContactGroupModel
    {
        public override long Id { get => 0; }
    }
}
