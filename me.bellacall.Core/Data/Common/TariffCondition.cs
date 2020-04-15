using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Условия тарифа
    /// </summary>
    public class TariffCondition : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        [ForeignKey("Tariff_Id")]
        public virtual Tariff Tariff { get; set; }
        public long Tariff_Id { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public DateTime DateStop { get; set; }

        /// <summary>
        /// Правило списания
        /// </summary>
        public TariffConditionRule Rule { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public TariffConditionUnit Unit { get; set; }

        /// <summary>
        /// Цена за единицу
        /// </summary>
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Единица измерения
    /// </summary>
    public enum TariffConditionUnit : int
    {
        /// <summary>
        /// Год
        /// </summary>
        Year = 1,

        /// <summary>
        /// Квартал
        /// </summary>
        Quarter = 2,

        /// <summary>
        /// Месяц
        /// </summary>
        Month = 3,

        /// <summary>
        /// Неделя
        /// </summary>
        Week = 4,

        /// <summary>
        /// Сутки
        /// </summary>
        Day = 5,

        /// <summary>
        /// Час
        /// </summary>
        Hour = 6,

        /// <summary>
        /// Минута
        /// </summary>
        Minute = 7,

        /// <summary>
        /// Секунда
        /// </summary>
        Second = 8,

        /// <summary>
        /// Миллисекунда
        /// </summary>
        Millisecond = 9,

        /// <summary>
        /// Человек
        /// </summary>
        Person = 10
    }

    /// <summary>
    /// Правило списания
    /// </summary>
    public enum TariffConditionRule : int
    {
        /// <summary>
        /// За период
        /// </summary>
        Period = 1,

        /// <summary>
        /// За трафик
        /// </summary>
        Traffic = 2,

        /// <summary>
        /// За лиды
        /// </summary>
        Lead = 3
    }
}
