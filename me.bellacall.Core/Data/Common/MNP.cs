using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Перенесенный мобильный номер
    /// </summary>
    public class MNP : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        [Required, StringLength(16)]
        // TODO: Index("IX_MNPs_Number", IsClustered = false, IsUnique = true)
        public string Number { get; set; }

        /// <summary>
        /// Оператор связи
        /// </summary>
        [ForeignKey("Provider_Id")]
        public virtual Provider Provider { get; set; }
        public long Provider_Id { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [ForeignKey("Region_Id")]
        public virtual Region Region { get; set; }
        public long Region_Id { get; set; }
    }
}
