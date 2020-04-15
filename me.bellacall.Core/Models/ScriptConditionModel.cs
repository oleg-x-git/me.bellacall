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
    /// Условный переход
    /// </summary>
    public class ScriptConditionModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Элемент
        /// </summary>
        [Log]
        public long ScriptElement_Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Log, StringLength(128), Required]
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
        [Log]
        public string CaseExpression { get; set; }

        /// <summary>
        /// Элемент варианта
        /// </summary>
        [Log]
        public long CaseElement_Id { get; set; }
    }

    /// <summary>
    /// Условный переход / create
    /// </summary>
    public class ScriptConditionCreateModel : ScriptConditionModel
    {
        public override long Id { get => 0; }
    }
}
