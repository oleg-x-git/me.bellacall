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
    /// Шлюз
    /// </summary>
    public class GatewayModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [Log]
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [Log, StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Тип регистрации
        /// </summary>
        [Log]
        public GatewayRegistrationType RegistrationType { get; set; }
    }

    /// <summary>
    /// Шлюз / create
    /// </summary>
    public class GatewayCreateModel : GatewayModel
    {
        public override long Id { get => 0; }
    }
}
