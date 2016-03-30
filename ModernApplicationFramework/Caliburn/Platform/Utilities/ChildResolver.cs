using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Caliburn.Platform.Utilities
{
    /// <summary>
    /// Represents a resolver that takes a control and returns it's children
    /// </summary>
    public class ChildResolver
    {
        private readonly Func<Type, bool> _filter;
        private readonly Func<DependencyObject, IEnumerable<DependencyObject>> _resolver;

        /// <summary>
        /// Creates the ChildResolver using the given anonymous methods.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <param name="resolver">The resolver</param>
        public ChildResolver(Func<Type, bool> filter, Func<DependencyObject, IEnumerable<DependencyObject>> resolver)
        {
            _filter = filter;
            _resolver = resolver;
        }

        /// <summary>
        /// Can this resolve appy to the given type.
        /// </summary>
        /// <param name="type">The visual tree type.</param>
        /// <returns>Returns true if this resolver applies.</returns>
        public bool CanResolve(Type type)
        {
            return _filter(type);
        }

        /// <summary>
        /// The element from the visual tree for the children to resolve.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<DependencyObject> Resolve(DependencyObject obj)
        {
            return _resolver(obj);
        }
    }

    /// <summary>
    /// Generic strongly typed child resolver
    /// </summary>
    /// <typeparam name="T">The type to filter on</typeparam>
    public class ChildResolver<T> : ChildResolver where T : DependencyObject
    {
        /// <summary>
        /// Creates a 
        /// </summary>
        /// <param name="resolver"></param>
        public ChildResolver(Func<T, IEnumerable<DependencyObject>> resolver) : base(
            t => typeof (T).IsAssignableFrom(t),
            o => resolver((T) o))
        {
        }
    }
}