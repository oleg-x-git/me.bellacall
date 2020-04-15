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
    /// Рассылка
    /// </summary>
    public class JobModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long Campaign_Id { get; set; }

        #region .. Параметры рассылки ..

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [Log]
        public long Region_Id { get; set; }

        /// <summary>
        /// Разрешить рассылку
        /// </summary>
        [Log]
        public bool AllowJob { get; set; }

        /// <summary>
        /// Разрешить коллбэки
        /// </summary>
        [Log]
        public bool AllowInbox { get; set; }

        /// <summary>
        /// Время запуска
        /// </summary>
        [Log]
        public TimeSpan TimeStart { get; set; }

        /// <summary>
        /// Время остановки
        /// </summary>
        [Log]
        public TimeSpan TimeStop { get; set; }

        /// <summary>
        /// Часовой пояс
        /// </summary>
        [Log]
        public JobTimeZoneType TimeZoneType { get; set; }

        /// <summary>
        /// Часовой пояс (код для Custom)
        /// </summary>
        [Log, StringLength(128)]
        public string TimeZoneCustom { get; set; }

        #endregion

        #region .. Параметры дозвона ..

        /// <summary>
        /// Продолжительность попытки дозвона, сек
        /// </summary>
        [Log]
        public int DialDuration { get; set; }

        /// <summary>
        /// Интенсивность дозвона, контактов в минуту
        /// </summary>
        [Log]
        public int DialDensity { get; set; }

        /// <summary>
        /// Количество попыток дозвона
        /// </summary>
        [Log]
        public int DialEfforts { get; set; }

        /// <summary>
        /// Интервал между попытками дозвона, минут
        /// </summary>
        [Log]
        public int DialInterval { get; set; }

        #endregion
    }

    /// <summary>
    /// Рассылка / create
    /// </summary>
    public class JobCreateModel : JobModel
    {
        public override long Id { get => 0; }
    }
}
