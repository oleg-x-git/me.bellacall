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
    /// Поток шлюза
    /// </summary>
    public class GatewayStreamModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Шлюз
        /// </summary>
        [Log]
        public long Gateway_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [Log, Required, StringLength(32)]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Log, Required, StringLength(32)]
        public string Password { get; set; }

        /// <summary>
        /// Тип линий
        /// </summary>
        [Log]
        public GatewayStreamTrunkType TrunkType { get; set; }

        /// <summary>
        /// Количество линий
        /// </summary>
        [Log]
        public int TrunkCount { get; set; }
    }

    /// <summary>
    /// Поток шлюза / create
    /// </summary>
    public class GatewayStreamCreateModel : GatewayStreamModel
    {
        public override long Id { get => 0; }
    }
}
