using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Контакт рассылки
    /// </summary>
    public class JobContactModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Рассылка
        /// </summary>
        [Log]
        public long Job_Id { get; set; }

        /// <summary>
        /// Контакт
        /// </summary>
        [Log]
        public long Contact_Id { get; set; }

        /// <summary>
        /// Состояние 
        /// </summary>
        [Log]
        public JobContactState State { get; set; }

        /// <summary>
        /// Дата/время последней попытки рассылки
        /// </summary>
        [Log]
        public DateTime? LastAttemptDate { get; set; }
    }

    /// <summary>
    /// Контакт рассылки / create
    /// </summary>
    public class JobContactCreateModel : JobContactModel
    {
        public override long Id { get => 0; }
    }
}
