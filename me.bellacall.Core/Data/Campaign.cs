using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Кампания
    /// </summary>
    public class Campaign : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [ForeignKey("Company_Id")]
        public virtual Company Company { get; set; }
        public long Company_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        [ForeignKey("Tariff_Id")]
        public virtual Common.Tariff Tariff { get; set; }
        public long Tariff_Id { get; set; }

        /// <summary>
        /// Свойства лида
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<LeadProperty> LeadProperties { get; set; }

        /// <summary>
        /// Свойства контакта
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<ContactProperty> ContactProperties { get; set; }

        /// <summary>
        /// Группы контактов
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<ContactGroup> ContactGroups { get; set; }

        /// <summary>
        /// Сценарии
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<Script> Scripts { get; set; }

        /// <summary>
        /// Рассылки
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<Job> Jobs { get; set; }

        /// <summary>
        /// Лиды
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<Lead> Leads { get; set; }

        /// <summary>
        /// Шлюзы
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<Gateway> Gateways { get; set; }

        /// <summary>
        /// Роли аккаунта
        /// </summary>
        [InverseProperty("Campaign")]
        public virtual IList<AspNetUserRole> UserRoles { get; set; }
    }
}
