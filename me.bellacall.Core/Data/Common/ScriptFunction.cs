using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Функция сценария
    /// </summary>
    public class ScriptFunction : IScriptListItem
    {
        public ScriptFunction() { Input = new List<ScriptFunctionParameter>(); Output = new List<ScriptFunctionParameter>(); }

        /// <summary>
        /// Имя функции
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Входные параметры по умолчанию
        /// </summary>
        public List<ScriptFunctionParameter> Input { get; set; }

        /// <summary>
        /// Выходные параметры по умолчанию
        /// </summary>
        public List<ScriptFunctionParameter> Output { get; set; }
    }

    /// <summary>
    /// Параметр функции сценария
    /// </summary>
    public class ScriptFunctionParameter
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип данных по умолчанию
        /// </summary>
        public ScriptDataType DataType { get; set; }
    }
}