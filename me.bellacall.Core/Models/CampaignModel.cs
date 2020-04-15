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
    /// Кампания
    /// </summary>
    public class CampaignModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [Log]
        public long Company_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        [Log]
        public long Tariff_Id { get; set; }
    }

    /// <summary>
    /// Кампания / create
    /// </summary>
    public class CampaignCreateModel : CampaignModel
    {
        public override long Id { get => 0; }
    }
}
