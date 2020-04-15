using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Параметр элемента сценария
    /// </summary>
    public class ScriptParameter : IEntity
    {
        public long Id { get; set; }

        public long ScriptElement_Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [StringLength(128), Required]
        public string Name { get; set; }

        /// <summary>
        /// Тип данных
        /// </summary>
        public ScriptDataType DataType { get; set; }

        /// <summary>
        /// Выражение
        /// 
        /// примеры для входных параметров:
        /// [Id]
        /// [Name] + '_x_1'
        /// 'is an example'
        /// 5 + 7 * (2 + [Count])
        /// 
        /// примеры для выходных параметров:
        /// [Name]
        /// [Email]
        /// 
        /// примеры для условий (true возвращает 1, false - 0):
        /// [Id] != 0
        /// [Name] like '%example%'
        /// [Count] > 42
        /// 
        /// </summary>
        public string Expression { get; set; }
    }

    /// <summary>
    /// Входной параметр элемента сценария
    /// </summary>
    public class ScriptInputParameter : ScriptParameter
    {
        /// <summary>
        /// Элемент
        /// </summary>
        [ForeignKey("ScriptElement_Id")]
        public virtual ScriptElement ScriptElement { get; set; }
    }

    /// <summary>
    /// Выходной параметр элемента сценария
    /// </summary>
    public class ScriptOutputParameter : ScriptParameter
    {
        /// <summary>
        /// Элемент
        /// </summary>
        [ForeignKey("ScriptElement_Id")]
        public virtual ScriptElement ScriptElement { get; set; }
    }
}
