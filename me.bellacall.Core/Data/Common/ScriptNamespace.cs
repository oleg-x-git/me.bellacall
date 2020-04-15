using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    public class ScriptNamespace : IScriptListItem
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цвет
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Список функций
        /// </summary>
        public ScriptList<ScriptFunction> Functions { get; set; }
    }

    /// <summary>
    /// Список пространств имен сценария
    /// </summary>
    public static class ScriptNamespaces
    {
        public static ScriptFunction GetFunction(string fullName)
        {
            var names = fullName.Split('.');
            return List[names[0]].Functions[names[1]];
        }

        public static ScriptNamespace GetNamespace(string fullName)
        {
            var names = fullName.Split('.');
            return List[names[0]];
        }

        public static ScriptList<ScriptNamespace> List = new ScriptList<ScriptNamespace>
        {
            new ScriptNamespace
            {
                Name = "Base",
                Color = "OliveDrab",
                Functions = ScriptFunctions.Base
            },

            new ScriptNamespace
            {
                Name = "Log",
                Color = "Gold",
                Functions = ScriptFunctions.Log
            },

            new ScriptNamespace
            {
                Name = "File",
                Color = "RoyalBlue",
                Functions = ScriptFunctions.File
            },

            new ScriptNamespace
            {
                Name = "Sound",
                Color = "Tomato",
                Functions = ScriptFunctions.Sound
            }
        };
    }
}
