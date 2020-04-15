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
    /// Переменная сценария
    /// </summary>
    public class ScriptVariableModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Сценарий
        /// </summary>
        [Log]
        public long Script_Id { get; set; }

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
    }

    /// <summary>
    /// Переменная сценария / create
    /// </summary>
    public class ScriptVariableCreateModel : ScriptVariableModel
    {
        public override long Id { get => 0; }
    }
}
