using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Лид
    /// </summary>
    public class LeadModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Свойства (json)
        /// </summary>
        [Log]
        public string Properties { get; set; }
    }

    /// <summary>
    /// Лид / create
    /// </summary>
    public class LeadCreateModel : LeadModel
    {
        public override long Id { get => 0; }
    }
}
