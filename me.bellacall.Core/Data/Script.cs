using me.bellacall.Core.Locales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Сценарий
    /// </summary>
    public class Script : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Свойства (статические переменные)
        /// </summary>
        [InverseProperty("Script")]
        public virtual IList<ScriptProperty> ScriptProperties { get; set; }

        /// <summary>
        /// Переменные
        /// </summary>
        [InverseProperty("Script")]
        public virtual IList<ScriptVariable> ScriptVariables { get; set; }

        /// <summary>
        /// Элементы
        /// </summary>
        [InverseProperty("Script")]
        public virtual IList<ScriptElement> ScriptElements { get; set; }
    }

    /// <summary>
    /// Тип данных сценария
    /// </summary>
    public enum ScriptDataType : int
    {
        /// <summary>
        /// Число
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ScriptDataType_Numeric")]
        Numeric = 1,

        /// <summary>
        /// Строка
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ScriptDataType_String")]
        String = 2,

        /// <summary>
        /// Дата и время
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ScriptDataType_DateTime")]
        DateTime = 3,

        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "ScriptDataType_Guid")]
        Guid = 4,
    }
}
