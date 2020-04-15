using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace me.bellacall.Core.Data.Common
{
    /// <summary>
    /// Элемент списка
    /// </summary>
    public interface IScriptListItem { string Name { get; set; } }

    /// <summary>
    /// Список с поиском по имени
    /// </summary>
    public class ScriptList<T> : List<T> where T : IScriptListItem
    {
        public ScriptList() : base() { }
        public ScriptList(IEnumerable<T> collection) : base(collection) { }
        public T this[string name] { get { return this.SingleOrDefault(e => e.Name == name); } }
    }
}
