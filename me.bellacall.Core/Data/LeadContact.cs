using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Контакт лида
    /// </summary>
    public class LeadContact : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Лид
        /// </summary>
        [ForeignKey("Lead_Id")]
        public virtual Lead Lead { get; set; }
        public long Lead_Id { get; set; }

        /// <summary>
        /// Контакт
        /// </summary>
        [ForeignKey("Contact_Id")]
        public virtual Contact Contact { get; set; }
        public long Contact_Id { get; set; }
    }
}
