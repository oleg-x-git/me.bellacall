using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using me.bellacall.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Свойство лида
    /// </summary>
    public class LeadPropertyModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Имя (json key)
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Caption { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [Log]
        public PropertyType Type { get; set; }
    }

    /// <summary>
    /// Свойство лида / create
    /// </summary>
    public class LeadPropertyCreateModel : LeadPropertyModel
    {
        public override long Id { get => 0; }
    }
}
