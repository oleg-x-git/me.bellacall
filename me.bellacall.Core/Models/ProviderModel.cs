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
    /// Оператор связи
    /// </summary>
    public class ProviderModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [Log, StringLength(32)]
        public string Code { get; set; }
    }

    /// <summary>
    /// Оператор связи / create
    /// </summary>
    public class ProviderCreateModel : ProviderModel
    {
        public override long Id { get => 0; }
    }
}
