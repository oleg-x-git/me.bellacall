using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    using Function = ScriptFunction;
    using Parameter = ScriptFunctionParameter;
    using Parameters = List<ScriptFunctionParameter>;

    public static partial class ScriptFunctions
    {
        /// <summary>
        /// Файловые операции
        /// </summary>
        public static ScriptList<Function> File = new ScriptList<Function>
        {
            new Function
            {
                Name = "Delete",
                Description = "Удаляет файл",
                Input = new Parameters { new Parameter { Name = "FileName", DataType = ScriptDataType.String } },
            }

            // ... и т.д.
        };
    }
}
