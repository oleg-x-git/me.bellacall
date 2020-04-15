using me.bellacall.Core.Controllers;
using me.bellacall.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Models
{
    /// <summary>
    /// Сценарий рассылки
    /// </summary>
    public class JobScriptModel : IModel
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
    /// Сценарий рассылки / create
    /// </summary>
    public class JobScriptCreateModel : JobScriptModel
    {
        public override long Id { get => 0; }
    }
}
