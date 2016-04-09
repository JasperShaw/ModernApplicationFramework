using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using ModernApplicationFramework.Caliburn.Collections;
using ModernApplicationFramework.Caliburn.Extensions;

namespace ModernApplicationFramework.Caliburn.Platform.Core
{
    /// <summary>
    ///     A source of assemblies that are inspectable by the framework.
    /// </summary>
    public static class AssemblySource
    {
        /// <summary>
        ///     The singleton instance of the AssemblySource used by the framework.
        /// </summary>
        public static readonly IObservableCollection<Assembly> Instance = new BindableCollection<Assembly>();

        /// <summary>
        ///     Finds a type which matches one of the elements in the sequence of names.
        /// </summary>
        public static Func<IEnumerable<string>, Type> FindTypeByNames = names =>
        {
            var type = names?.Join(Instance.SelectMany(a => a.GetExportedTypes()), n => n, t => t.FullName, (n, t) => t)
                             .FirstOrDefault();
            return type;
        };
    }

    /// <summary>
    ///     A caching subsystem for <see cref="AssemblySource" />.
    /// </summary>
    public static class AssemblySourceCache
    {
        /// <summary>
        ///     Extracts the types from the spezified assembly for storing in the cache.
        /// </summary>
        public static Func<Assembly, IEnumerable<Type>> ExtractTypes = assembly =>
            assembly.GetExportedTypes()
                    .Where(t =>
                        typeof(UIElement).IsAssignableFrom(t) ||
                        typeof(INotifyPropertyChanged).IsAssignableFrom(t));

        private static readonly IDictionary<string, Type> TypeNameCache = new Dictionary<string, Type>();
        private static bool _isInstalled;

        /// <summary>
        ///     Installs the caching subsystem.
        /// </summary>
        public static void Install()
        {
            if (_isInstalled)
                return;
            _isInstalled = true;

            AssemblySource.Instance.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        e.NewItems.OfType<Assembly>()
                         .SelectMany(a => ExtractTypes(a))
                         .Apply(t => TypeNameCache.Add(t.FullName, t));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        TypeNameCache.Clear();
                        AssemblySource.Instance
                                      .SelectMany(a => ExtractTypes(a))
                                      .Apply(t => TypeNameCache.Add(t.FullName, t));
                        break;
                }
            };

            AssemblySource.Instance.Refresh();

            AssemblySource.FindTypeByNames = names =>
            {
                var type = names?.Select(n => TypeNameCache.GetValueOrDefault(n)).FirstOrDefault(t => t != null);
                return type;
            };
        }
    }
}