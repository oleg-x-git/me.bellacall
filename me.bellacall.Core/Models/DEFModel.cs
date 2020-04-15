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
    /// Код мобильного оператора
    /// </summary>
    public class DEFModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [Log, Required, StringLength(16)]
        public string Code { get; set; }

        /// <summary>
        /// Оператор связи
        /// </summary>
        [Log]
        public long Provider_Id { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [Log]
        public long Region_Id { get; set; }
    }

    /// <summary>
    /// Код мобильного оператора / create
    /// </summary>
    public class DEFCreateModel : DEFModel
    {
        public override long Id { get => 0; }
    }
}
