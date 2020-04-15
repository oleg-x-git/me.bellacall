using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Контакт рассылки
    /// </summary>
    public class JobContact : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Рассылка
        /// </summary>
        [ForeignKey("Job_Id")]
        public virtual Job Job { get; set; }
        public long Job_Id { get; set; }

        /// <summary>
        /// Контакт
        /// </summary>
        [ForeignKey("Contact_Id")]
        public virtual Contact Contact { get; set; }
        public long Contact_Id { get; set; }

        /// <summary>
        /// Состояние 
        /// </summary>
        public JobContactState State { get; set; }

        /// <summary>
        /// Дата/время последней попытки рассылки
        /// </summary>
        public DateTime? LastAttemptDate { get; set; }
    }

    /// <summary>
    /// Состояние рассылки контакта
    /// </summary>
    public enum JobContactState : int
    {
        /// <summary>
        /// Новый
        /// </summary>
        New = 0,

        /// <summary>
        /// Обрабатывается
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Успешно
        /// </summary>
        Success = 2,

        /// <summary>
        /// Сброс
        /// </summary>
        Reject = 3,

        /// <summary>
        /// Неудача
        /// </summary>
        Fail = 4
    }
}
