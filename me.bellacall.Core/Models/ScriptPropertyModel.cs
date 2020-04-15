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
    /// Свойство сценария
    /// </summary>
    public class ScriptPropertyModel : IModel
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

        /// <summary>
        /// Значение.
        /// Правило преобразования в строку: convert(nvarchar(max), @value, 20)
        /// Value is null == значение по умолчанию для заданного типа
        /// </summary>
        [Log]
        public string Value { get; set; }
    }

    /// <summary>
    /// Свойство сценария / create
    /// </summary>
    public class ScriptPropertyCreateModel : ScriptPropertyModel
    {
        public override long Id { get => 0; }
    }
}
