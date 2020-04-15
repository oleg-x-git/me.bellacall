using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Сценарий коллбэка
    /// </summary>
    public class InboxScriptModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Рассылка
        /// </summary>
        [Log]
        public long Job_Id { get; set; }

        /// <summary>
        /// Сценарий
        /// </summary>
        [Log]
        public long Script_Id { get; set; }
    }

    /// <summary>
    /// Сценарий коллбэка / create
    /// </summary>
    public class InboxScriptCreateModel : InboxScriptModel
    {
        public override long Id { get => 0; }
    }
}
