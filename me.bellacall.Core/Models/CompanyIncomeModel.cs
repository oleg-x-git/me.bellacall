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
    /// Начисление средств
    /// </summary>
    public class CompanyIncomeModel : IModel
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
        /// Тип
        /// </summary>
        [Log]
        public CompanyIncomeType Type { get; set; }

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
    /// Начисление средств / create
    /// </summary>
    public class CompanyIncomeCreateModel : CompanyIncomeModel
    {
        public override long Id { get => 0; }
    }
}
