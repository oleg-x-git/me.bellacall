using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Контакт лида
    /// </summary>
    public class LeadContactModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Лид
        /// </summary>
        [Log]
        public long Lead_Id { get; set; }

        /// <summary>
        /// Контакт
        /// </summary>
        [Log]
        public long Contact_Id { get; set; }
    }

    /// <summary>
    /// Контакт лида / create
    /// </summary>
    public class LeadContactCreateModel : LeadContactModel
    {
        public override long Id { get => 0; }
    }
}
