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
        /// Базовые функции
        /// </summary>
        public static ScriptList<Function> Base = new ScriptList<Function>
        {
            // Входные параметры - параметры сценария. Устанавливает значения переменных через выходные параметры (аналогично элементу SetValue).
            new Function
            {
                Name = "Start",
                Description = "Первый элемент сценария"
            },
            
            // Выходные параметры возвращаются в вызывающий сценарий.
            new Function
            {
                Name = "Stop",
                Description = "Выход из сценария"
            },

            // Перенаправляет значения входных параметров в значения одноименных выходных параметров.
            new Function
            {
                Name = "SetValue",
                Description = "Устанавливает значения переменных"
            },

            new Function
            {
                Name = "Switch",
                Description = "Выбор следующего элемента по условию",
                Input = new Parameters { new Parameter { Name = "Condition", DataType = ScriptDataType.String } },
                Output = new Parameters { new Parameter { Name = "Condition", DataType = ScriptDataType.Numeric } }
            },

            // Входные параметры передаются компоненту "Start" вызываемого параметра.
            // Выходные параметры устанавливаются из одноименных выходных параметров компонента "Stop" вызываемого сценария (для синхронных вызовов).
            new Function
            {
                Name = "Script",
                Description = "Вызов сценария",
                Input = new Parameters { new Parameter { Name = "Async", DataType = ScriptDataType.Numeric } }
            },

            // ... и т.д.
        };
    }
}
