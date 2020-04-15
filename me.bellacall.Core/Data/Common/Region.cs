using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Регион
    /// </summary>
    public class Region : IEntity
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
        /// Часовой пояс
        /// </summary>
        [Required, StringLength(128)]
        public string TimeZone { get; set; }

        /// <summary>
        /// Коды DEF
        /// </summary>
        [InverseProperty("Region")]
        public virtual IList<DEF> DEFs { get; set; }

        /// <summary>
        /// Номера MNP
        /// </summary>
        [InverseProperty("Region")]
        public virtual IList<MNP> MNPs { get; set; }
    }
}
