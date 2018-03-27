using System;
using System.Collections.Generic;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.Interfaces.Services;

namespace ModernApplicationFramework.EditorBase.Services
{
    public abstract class UniqueNameCreator<T> : IUniqueNameCreator<T> where T : IExtensionDefinition
    {
        private readonly Dictionary<Type, int> _lookupDic = new Dictionary<Type, int>();

        /// <inheritdoc />
        /// <summary>
        ///  Returns an unique name in this application instance.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The unique name</returns>
        public virtual string GetUniqueName(T extension)
        {
            var t = extension.GetType();
            if (!_lookupDic.ContainsKey(t))
                _lookupDic.Add(t, 0);
            var number = ++_lookupDic[t];
            return $"{extension.TemplateName}{number}";
        }

        /// <inheritdoc />
        /// <summary>
        /// Flushes the store.
        /// </summary>
        public void Flush()
        {
            _lookupDic.Clear();
        }
    }
}
