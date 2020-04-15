using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Сценарий рассылки
    /// </summary>
    public class JobScript : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Рассылка
        /// </summary>
        [ForeignKey("Job_Id")]
        public virtual Job Job { get; set; }
        public long Job_Id { get; set; }
        
        /// <summary>
        /// Сценарий
        /// </summary>
        [ForeignKey("Script_Id")]
        public virtual Script Script { get; set; }
        public long Script_Id { get; set; }
    }
}
