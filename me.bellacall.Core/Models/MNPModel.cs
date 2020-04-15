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
    /// Перенесенный мобильный номер
    /// </summary>
    public class MNPModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        [Log, Required, StringLength(16)]
        public string Number { get; set; }

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
    /// Перенесенный мобильный номер / create
    /// </summary>
    public class MNPCreateModel : MNPModel
    {
        public override long Id { get => 0; }
    }
}
