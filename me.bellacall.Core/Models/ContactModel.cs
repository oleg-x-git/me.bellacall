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
    /// Контакт
    /// </summary>
    public class ContactModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        [Log]
        public long ContactGroup_Id { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        [Log, Required, StringLength(32)]
        public string Phone { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [Log, StringLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        [Log]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        [Log]
        public ContactGender? Gender { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        [Log, StringLength(32)]
        public string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Log, StringLength(32)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [Log, StringLength(32)]
        public string Patronymic { get; set; }

        /// <summary>
        /// Дополнительные свойства (json)
        /// </summary>
        [Log]
        public string Properties { get; set; }

        /// <summary>
        /// Статус (null - не актуализирован)
        /// </summary>
        [Log]
        public ContactStatus? Status { get; set; }
    }

    /// <summary>
    /// Контакт / create
    /// </summary>
    public class ContactCreateModel : ContactModel
    {
        public override long Id { get => 0; }
    }
}
