using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Тариф
    /// </summary>
    public class Tariff : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Доступность
        /// </summary>
        public bool AllowEnjoy { get; set; }

        /// <summary>
        /// Условия
        /// </summary>
        [InverseProperty("Tariff")]
        public virtual IList<TariffCondition> TariffConditions { get; set; }
    }
}
