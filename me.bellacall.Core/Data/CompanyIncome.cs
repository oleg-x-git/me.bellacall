using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Начисление средств
    /// </summary>
    public class CompanyIncome : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [ForeignKey("Company_Id")]
        public virtual Company Company { get; set; }
        public long Company_Id { get; set; }

        /// <summary>
        /// Дата/время
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public CompanyIncomeType Type { get; set; }

        /// <summary>
        /// Сумма, ₽ / $ / € / ...
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        [StringLength(256)]
        public string Note { get; set; }
    }

    /// <summary>
    /// Тип операции начисления
    /// </summary>
    public enum CompanyIncomeType : int
    {
        /// <summary>
        /// Эквайринг
        /// </summary>
        Acquiring = 1
    }
}
