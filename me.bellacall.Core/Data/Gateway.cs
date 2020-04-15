using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Шлюз
    /// </summary>
    public class Gateway : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        [ForeignKey("Campaign_Id")]
        public virtual Campaign Campaign { get; set; }
        public long Campaign_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(256)]
        public string Description { get; set; }

        /// <summary>
        /// Тип регистрации
        /// </summary>
        public GatewayRegistrationType RegistrationType { get; set; }

        /// <summary>
        /// Потоки
        /// </summary>
        [InverseProperty("Gateway")]
        public virtual IList<GatewayStream> GatewayStreams { get; set; }
    }

    /// <summary>
    /// Тип регистрации
    /// </summary>
    public enum GatewayRegistrationType
    {
        /// <summary>
        /// Без регистрации
        /// </summary>
        Without = 1,

        /// <summary>
        /// На стороне сервера
        /// </summary>
        OnServer = 2,

        /// <summary>
        /// На стороне шлюза
        /// </summary>
        OnGateway = 3
    }
}
