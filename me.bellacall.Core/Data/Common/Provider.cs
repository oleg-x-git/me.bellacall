using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Оператор связи
    /// </summary>
    public class Provider : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [StringLength(32)]
        public string Code { get; set; }

        /// <summary>
        /// Коды DEF
        /// </summary>
        [InverseProperty("Provider")]
        public virtual IList<DEF> DEFs { get; set; }

        /// <summary>
        /// Номера MNP
        /// </summary>
        [InverseProperty("Provider")]
        public virtual IList<MNP> MNPs { get; set; }
    }
}
