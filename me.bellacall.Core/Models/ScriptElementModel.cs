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
    /// Элемент сценария
    /// </summary>
    public class ScriptElementModel : IModel
    {
        public virtual long Id { get; set; }

        /// <summary>
        /// Сценарий
        /// </summary>
        [Log]
        public long Script_Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        [Log]
        public string Description { get; set; }

        /// <summary>
        /// Имя функции
        /// </summary>
        [Log, Required, StringLength(128)]
        public string Function { get; set; }

        /// <summary>
        /// Координата X
        /// </summary>
        [Log]
        public int XPos { get; set; }

        /// <summary>
        /// Координата Y
        /// </summary>
        [Log]
        public int YPos { get; set; }
    }

    /// <summary>
    /// Элемент сценария / create
    /// </summary>
    public class ScriptElementCreateModel : ScriptElementModel
    {
        public override long Id { get => 0; }
    }
}
