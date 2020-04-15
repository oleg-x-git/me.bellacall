using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Условный переход к элементу сценария
    /// </summary>
    public class ScriptCondition : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Элемент
        /// </summary>
        [ForeignKey("ScriptElement_Id")]
        public virtual ScriptElement ScriptElement { get; set; }
        public long ScriptElement_Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [StringLength(128), Required]
        public string Name { get; set; }

        /// <summary>
        /// Значение варианта (параметр Condition)
        /// 
        /// примеры:
        /// 5
        /// 'yes'
        /// [Index]
        /// 
        /// пустая строка - безусловный переход
        /// 
        /// </summary>
        public string CaseExpression { get; set; }

        /// <summary>
        /// Элемент варианта
        /// </summary>
        [ForeignKey("CaseElement_Id")]
        public virtual ScriptElement CaseElement { get; set; }
        public long CaseElement_Id { get; set; }
    }
}
