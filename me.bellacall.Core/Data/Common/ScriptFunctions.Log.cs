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
        /// Голосовые операции
        /// </summary>
        public static ScriptList<Function> Log = new ScriptList<Function>
        {
            new Function
            {
                Name = "Step",
                Description = "Добавляет в лог отметку о прохождении",
                Input = new Parameters
                {
                    new Parameter { Name = "Name", DataType = ScriptDataType.String },
                    new Parameter { Name = "Expression", DataType = ScriptDataType.String },
                    new Parameter { Name = "Value", DataType = ScriptDataType.String }
                }
            },

            new Function
            {
                Name = "Debug",
                Description = "Добавляет в лог отладочную информацию",
                Input = new Parameters { new Parameter { Name = "Text", DataType = ScriptDataType.String } }
            }

            // ... и т.д.
        };
    }
}
