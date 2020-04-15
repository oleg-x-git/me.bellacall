using me.bellacall.Core.Locales;
using System.ComponentModel.DataAnnotations;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Свойство
    /// </summary>
    public abstract class Property : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Имя (json key)
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        [Required, StringLength(128)]
        public string Caption { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public PropertyType Type { get; set; }
    }

    /// <summary>
    /// Тип свойства
    /// </summary>
    public enum PropertyType : int
    {
        /// <summary>
        /// Число
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PropertyType_Numeric")]
        Numeric = 1,

        /// <summary>
        /// Строка
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PropertyType_String")]
        String = 2,

        /// <summary>
        /// Чек
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PropertyType_Boolean")]
        Boolean = 3,

        /// <summary>
        /// Дата
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PropertyType_Date")]
        Date = 4,

        /// <summary>
        /// Время
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "PropertyType_Time")]
        Time = 5
    }
}
