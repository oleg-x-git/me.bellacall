using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Списание средств
    /// </summary>
    public class CompanyExpense : IEntity
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
        /// Основание списания (условие тарифа)
        /// </summary>
        [ForeignKey("TariffCondition_Id")]
        public virtual Common.TariffCondition TariffCondition { get; set; }
        public long TariffCondition_Id { get; set; }

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
}
