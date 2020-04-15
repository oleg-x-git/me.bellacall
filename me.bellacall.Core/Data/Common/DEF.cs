using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Код мобильного оператора
    /// </summary>
    public class DEF : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [Required, StringLength(16)]
        // TODO: Index("IX_DEFs_Code", IsClustered = false, IsUnique = true)
        public string Code { get; set; }

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
