using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using me.bellacall.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Условие тарифа
    /// </summary>
    public class TariffConditionModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        [Log]
        public long Tariff_Id { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        [Log]
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        [Log]
        public DateTime DateStop { get; set; }

        /// <summary>
        /// Правило списания
        /// </summary>
        [Log]
        public TariffConditionRule Rule { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        [Log]
        public TariffConditionUnit Unit { get; set; }

        /// <summary>
        /// Цена за единицу
        /// </summary>
        [Log]
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Условие тарифа / create
    /// </summary>
    public class TariffConditionCreateModel : TariffConditionModel
    {
        public override long Id { get => 0; }
    }
}
