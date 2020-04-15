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
    /// Параметр элемента
    /// </summary>
    public class ScriptParameterModel : IModel
    {
        public virtual long Id { get; set; }

        [Log]
        public long ScriptElement_Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Log, StringLength(128), Required]
        public string Name { get; set; }

        /// <summary>
        /// Тип данных
        /// </summary>
        [Log]
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
        [Log]
        public string Expression { get; set; }
    }

    /// <summary>
    /// Параметр элемента / create
    /// </summary>
    public class ScriptParameterCreateModel : ScriptParameterModel
    {
        public override long Id { get => 0; }
    }
}
