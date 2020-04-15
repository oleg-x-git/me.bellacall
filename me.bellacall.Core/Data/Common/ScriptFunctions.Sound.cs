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
        public static ScriptList<Function> Sound = new ScriptList<Function>
        {
            new Function
            {
                Name = "Play",
                Description = "Воспроизводит звуковой файл",
                Input = new Parameters { new Parameter { Name = "SoundFileName", DataType = ScriptDataType.String } },
            },

            new Function
            {
                Name = "Recognize",
                Description = "Распознает ответ абонента",
                Input = new Parameters { new Parameter { Name = "SoundFileName", DataType = ScriptDataType.String } },
                Output = new Parameters
                {
                    new Parameter { Name = "Expression", DataType = ScriptDataType.String },
                    new Parameter { Name = "Value", DataType = ScriptDataType.String },
                }
            }

            // ... и т.д.
        };
    }
}
