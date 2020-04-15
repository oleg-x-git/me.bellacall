using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Элемент сценария
    /// </summary>
    public class ScriptElement : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Сценарий
        /// </summary>
        [ForeignKey("Script_Id")]
        public virtual Script Script { get; set; }
        public long Script_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Имя функции
        /// </summary>
        [Required, StringLength(128)]
        public string Function { get; set; }

        /// <summary>
        /// Координата X
        /// </summary>
        public int XPos { get; set; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public int YPos { get; set; }

        /// <summary>
        /// Входные параметры (передаются в одноименные параметры функции)
        /// </summary>
        [InverseProperty("ScriptElement")]
        public virtual IList<ScriptInputParameter> ScriptInputParameters { get; set; }

        /// <summary>
        /// Выходные параметры (устанавливают значения переменных)
        /// </summary>
        [InverseProperty("ScriptElement")]
        public virtual IList<ScriptOutputParameter> ScriptOutputParameters { get; set; }

        /// <summary>
        /// Условные переходы (по значению выходного параметра с именем Condition)
        /// </summary>
        [InverseProperty("ScriptElement")]
        public virtual IList<ScriptCondition> ScriptConditions { get; set; }
    }
}
