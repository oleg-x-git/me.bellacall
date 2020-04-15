using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Свойство контакта
    /// </summary>
    public class ContactProperty : Common.Property
    {
        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }
    }
}
