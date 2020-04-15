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
    /// Тариф
    /// </summary>
    public class TariffModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [Log]
        public string Description { get; set; }

        /// <summary>
        /// Доступность
        /// </summary>
        [Log]
        public bool AllowEnjoy { get; set; }
    }

    /// <summary>
    /// Тариф / create
    /// </summary>
    public class TariffCreateModel : TariffModel
    {
        public override long Id { get => 0; }
    }
}
