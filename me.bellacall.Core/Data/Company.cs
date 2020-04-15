using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Клиент
    /// </summary>
    public class Company : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// ФИО / Название
        /// </summary>
        [Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Аккаунты
        /// </summary>
        [InverseProperty("Company")]
        public virtual IList<AspNetUser> Users { get; set; }

        /// <summary>
        /// Кампании
        /// </summary>
        [InverseProperty("Company")]
        public virtual IList<Campaign> Campaigns { get; set; }

        /// <summary>
        /// Начисления средств
        /// </summary>
        [InverseProperty("Company")]
        public virtual IList<CompanyIncome> CompanyIncomes { get; set; }

        /// <summary>
        /// Списания средств
        /// </summary>
        [InverseProperty("Company")]
        public virtual IList<CompanyExpense> CompanyExpenses { get; set; }
    }
}
