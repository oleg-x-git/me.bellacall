using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Контакт
    /// </summary>
    public class Contact : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        [ForeignKey("ContactGroup_Id")]
        public virtual ContactGroup ContactGroup { get; set; }
        public long ContactGroup_Id { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Required, StringLength(32)]
        public string Phone { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [StringLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public ContactGender? Gender { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [StringLength(32)]
        public string Patronymic { get; set; }

        /// <summary>
        /// Дополнительные свойства (json)
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// Статус (null - не актуализирован)
        /// </summary>
        public ContactStatus? Status { get; set; }

        /// <summary>
        /// Контакты лидов
        /// </summary>
        [InverseProperty("Contact")]
        public virtual IList<LeadContact> LeadContacts { get; set; }

        /// <summary>
        /// Контакты рассылок
        /// </summary>
        [InverseProperty("Contact")]
        public virtual IList<JobContact> JobContacts { get; set; }
    }

    /// <summary>
    /// Пол контакта
    /// </summary>
    public enum ContactGender : int
    {
        /// <summary>
        /// Мужской
        /// </summary>
        Male = 1,

        /// <summary>
        /// Женский
        /// </summary>
        Female = 2
    }

    /// <summary>
    /// Статус контакта
    /// </summary>
    [Flags]
    public enum ContactStatus: int
    {
        None = 0,

        /// <summary>
        /// В рассылках
        /// </summary>
        OnJob = 1,

        /// <summary>
        /// В лидах
        /// </summary>
        OnLead = 2,

        /// <summary>
        /// В стоп-листе (для job)
        /// </summary>
        StopList = 1024,

        /// <summary>
        /// В черном списке (для inbox)
        /// </summary>
        BlackList = 2048,

        /// <summary>
        /// Недоступен (недозвон по актуализации)
        /// </summary>
        OnFail = 65536
    }
}
