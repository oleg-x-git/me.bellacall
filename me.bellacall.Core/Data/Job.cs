using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Рассылка
    /// </summary>
    public class Job : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }

        #region .. Параметры рассылки ..

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [ForeignKey("Region_Id")]
        public virtual Common.Region Region { get; set; }
        public long Region_Id { get; set; }

        /// <summary>
        /// Разрешить рассылку
        /// </summary>
        public bool AllowJob { get; set; }

        /// <summary>
        /// Разрешить коллбэки
        /// </summary>
        public bool AllowInbox { get; set; }

        /// <summary>
        /// Время запуска
        /// </summary>
        public TimeSpan TimeStart { get; set; }

        /// <summary>
        /// Время остановки
        /// </summary>
        public TimeSpan TimeStop { get; set; }

        /// <summary>
        /// Часовой пояс
        /// </summary>
        public JobTimeZoneType TimeZoneType { get; set; }

        /// <summary>
        /// Часовой пояс (код для Custom)
        /// </summary>
        [StringLength(128)]
        public string TimeZoneCustom { get; set; }

        #endregion

        #region .. Параметры дозвона ..

        /// <summary>
        /// Продолжительность попытки дозвона, сек
        /// </summary>
        public int DialDuration { get; set; }

        /// <summary>
        /// Интенсивность дозвона, контактов в минуту
        /// </summary>
        public int DialDensity { get; set; }

        /// <summary>
        /// Количество попыток дозвона
        /// </summary>
        public int DialEfforts { get; set; }

        /// <summary>
        /// Интервал между попытками дозвона, минут
        /// </summary>
        public int DialInterval { get; set; }

        #endregion

        /// <summary>
        /// Контакты
        /// </summary>
        [InverseProperty("Job")]
        public virtual IList<JobContact> JobContacts { get; set; }

        /// <summary>
        /// Сценарии рассылки
        /// </summary>
        [InverseProperty("Job")]
        public virtual IList<JobScript> JobScripts { get; set; }

        /// <summary>
        /// Сценарии коллбэков
        /// </summary>
        [InverseProperty("Job")]
        //[NotMapped]
        public virtual IList<InboxScript> InboxScripts { get; set; }
    }

    public enum JobTimeZoneType : int
    {
        /// <summary>
        /// Часовой пояс пользователя (клиента)
        /// </summary>
        Company = 1,

        /// <summary>
        /// Часовой пояс контакта (абонента)
        /// </summary>
        Contact = 2,

        /// <summary>
        /// Из списка
        /// </summary>
        Custom = 3
    }
}
