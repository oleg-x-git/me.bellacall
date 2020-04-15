using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Переменная сценария
    /// </summary>
    public class ScriptVariable : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Сценарий
        /// </summary>
        [ForeignKey("Script_Id")]
        public virtual Script Script { get; set; }
        public long Script_Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [StringLength(128), Required]
        public string Name { get; set; }

        /// <summary>
        /// Тип данных
        /// </summary>
        public ScriptDataType DataType { get; set; }
    }
}
