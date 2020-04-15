using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data
{
    /// <summary>
    /// Поток шлюза
    /// </summary>
    public class GatewayStream : IEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// Шлюз
        /// </summary>
        [ForeignKey("Gateway_Id")]
        public virtual Gateway Gateway { get; set; }
        public long Gateway_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [Required, StringLength(32)]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required, StringLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// Тип линий
        /// </summary>
        public GatewayStreamTrunkType TrunkType { get; set; }

        /// <summary>
        /// Количество линий
        /// </summary>
        public int TrunkCount { get; set; }
    }

    /// <summary>
    /// Тип линий потока
    /// </summary>
    public enum GatewayStreamTrunkType : int
    {
        /// <summary>
        /// Внешние
        /// </summary>
        External = 1,

        /// <summary>
        /// Внутрениие
        /// </summary>
        Internal = 2
    }
}
