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
    /// Списание средств
    /// </summary>
    public class CompanyExpenseModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [Log]
        public long Company_Id { get; set; }

        /// <summary>
        /// Дата/время
        /// </summary>
        [Log]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Основание списания (условие тарифа)
        /// </summary>
        [Log]
        public long TariffCondition_Id { get; set; }

        /// <summary>
        /// Сумма, ₽ / $ / € / ...
        /// </summary>
        [Log]
        public decimal Amount { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        [Log, StringLength(256)]
        public string Note { get; set; }
    }

    /// <summary>
    /// Списание средств / create
    /// </summary>
    public class CompanyExpenseCreateModel : CompanyExpenseModel
    {
        public override long Id { get => 0; }
    }
}
