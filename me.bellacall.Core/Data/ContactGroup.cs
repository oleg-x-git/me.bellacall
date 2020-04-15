using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Группа контактов
    /// </summary>
    public class ContactGroup : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Группы контактов
        /// </summary>
        [InverseProperty("ContactGroup")]
        public virtual IList<Contact> Contacts { get; set; }
    }
}
