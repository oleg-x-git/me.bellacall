using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Лид
    /// </summary>
    public class Lead : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Свойства (json)
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        [InverseProperty("Lead")]
        public virtual IList<LeadContact> LeadContacts { get; set; }
    }
}
