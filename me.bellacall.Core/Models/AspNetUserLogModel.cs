using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    public class AspNetUserLogModel : IModel
    {
        public long Id { get; set; }

        /// <summary>
        /// Аккаунт
        /// </summary>
        public long User_Id { get; set; }

        /// <summary>
        /// Кампания
        /// </summary>
        public long? Campaign_Id { get; set; }

        /// <summary>
        /// Дата/время
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Таблица
        /// </summary>
        [Required, StringLength(50)]
        public string TableName { get; set; }

        /// <summary>
        /// Операция
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        public Guid Operation_Id { get; set; }

        /// <summary>
        /// Идентификатор редактируемой строки
        /// </summary>
        public long? Data_Id { get; set; }

        /// <summary>
        /// Новые значяения редактируемой строки
        /// </summary>
        public string Data_Json { get; set; }
    }
}
