using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Platform.Utilities;
using ModernApplicationFramework.Caliburn.Platform.Xaml;

namespace ModernApplicationFramework.Caliburn.Platform
{
    /// <summary>
    ///     Provides methods for searching a given scope for named elements.
    /// </summary>
    public static class BindingScope
    {
        /// <summary>
        ///     Gets all the <see cref="FrameworkElement" /> instances with names in the scope.
        /// </summary>
        /// <returns>Named <see cref="FrameworkElement" /> instances in the provided scope.</returns>
        /// <remarks>
        ///     Pass in a <see cref="DependencyObject" /> and receive a list of named <see cref="FrameworkElement" />
        ///     instances in the same scope.
        /// </remarks>
        public static Func<DependencyObject, IEnumerable<FrameworkElement>> GetNamedElements = elementInScope =>
        {
            var routeHops = FindScopeNamingRoute(elementInScope);
            return FindNamedDescendants(routeHops);
        };

        private static readonly List<ChildResolver> ChildResolvers = new List<ChildResolver>();
        private static readonly Dictionary<Type, object> NonResolvableChildTypes = new Dictionary<Type, object>();

        /// <summary>
        ///     Finds a set of named <see cref="FrameworkElement" /> instances in each hop in a <see cref="ScopeNamingRoute" />.
        /// </summary>
        /// <remarks>
        ///     Searches all the elements in the <see cref="ScopeNamingRoute" /> parameter as well as the visual children of
        ///     each of these elements, the <see cref="ContentControl.Content" />, the <c>HeaderedContentControl.Header</c>,
        ///     the <see cref="ItemsControl.Items" />, or the <c>HeaderedItemsControl.Header</c>, if any are found.
        /// </remarks>
        public static Func<ScopeNamingRoute, IEnumerable<FrameworkElement>> FindNamedDescendants = routeHops =>
        {
            if (routeHops == null)
            {
                throw new ArgumentNullException(nameof(routeHops));
            }

            if (routeHops.Root == null)
            {
                throw new ArgumentException($"Root is null on the given {typeof(ScopeNamingRoute)}");
            }

            var descendants = new List<FrameworkElement>();
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(routeHops.Root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentElement = current as FrameworkElement;

                if (!string.IsNullOrEmpty(currentElement?.Name))
                    descendants.Add(currentElement);

                if (current is UserControl && !ReferenceEquals(current, routeHops.Root))
                    continue;

                DependencyObject hopTarget;
                if (routeHops.TryGetHop(current, out hopTarget))
                {
                    queue.Enqueue(hopTarget);
                    continue;
                }
                var childCount = current is Visual || current is Visual3D
                    ? VisualTreeHelper.GetChildrenCount(current)
                    : 0;
                if (childCount > 0)
                {
                    for (var i = 0; i < childCount; i++)
                    {
                        var childDo = VisualTreeHelper.GetChild(current, i);
                        queue.Enqueue(childDo);
                    }
                }
                else
                {
                    var currentType = current.GetType();

                    if (!NonResolvableChildTypes.ContainsKey(currentType))
                    {
                        var resolvers = ChildResolvers.Where(r => r.CanResolve(currentType)).ToArray();

                        if (!resolvers.Any())
                        {
                            NonResolvableChildTypes[currentType] = null;
                        }
                        else
                        {
                            resolvers
                                .SelectMany(r => r.Resolve(current) ?? Enumerable.Empty<DependencyObject>())
                                .Where(c => c != null)
                                .Apply(queue.Enqueue);
                        }
                    }
                }
            }

            return descendants;
        };

        /// <summary>
        ///     Finds a path of dependency objects which traces through visual anscestry until a root which is
        ///     <see langword="null" />,
        ///     a <see cref="UserControl" />, a <c>Page</c> with a dependency object <c>Page.ContentProperty</c> value,
        ///     a dependency object with <see cref="Xaml.View.IsScopeRootProperty" /> set to <see langword="true" />.
        ///     <see cref="ContentPresenter" />
        ///     and <see cref="ItemsPresenter" /> are included in the resulting <see cref="ScopeNamingRoute" /> in order to track
        ///     which item
        ///     in an items control we are scoped to.
        /// </summary>
        public static Func<DependencyObject, ScopeNamingRoute> FindScopeNamingRoute = elementInScope =>
        {
            var root = elementInScope;
            var previous = elementInScope;
            DependencyObject contentPresenter = null;
            var routeHops = new ScopeNamingRoute();

            while (true)
            {
                if (root == null)
                {
                    root = previous;
                    break;
                }

                if (root is UserControl)
                    break;
#if !SILVERLIGHT
                if (root is Page)
                {
                    root = ((Page) root).Content as DependencyObject ?? root;
                    break;
                }
#endif
                if ((bool) root.GetValue(View.IsScopeRootProperty))
                    break;

                if (root is ContentPresenter)
                    contentPresenter = root;
                else
                    if (root is ItemsPresenter && contentPresenter != null)
                    {
                        routeHops.AddHop(root, contentPresenter);
                        contentPresenter = null;
                    }

                previous = root;
                root = VisualTreeHelper.GetParent(previous);
            }

            routeHops.Root = root;
            return routeHops;
        };


        static BindingScope()
        {
            AddChildResolver<ContentControl>(e => new[] {e.Content as DependencyObject});
            AddChildResolver<ItemsControl>(e => e.Items.OfType<DependencyObject>().ToArray());
#if !SILVERLIGHT && !WinRT
            AddChildResolver<HeaderedContentControl>(e => new[] {e.Header as DependencyObject});
            AddChildResolver<HeaderedItemsControl>(e => new[] {e.Header as DependencyObject});
#endif
        }

        /// <summary>
        ///     Adds a child resolver.
        /// </summary>
        /// <param name="filter">The type filter.</param>
        /// <param name="resolver">The resolver.</param>
        public static ChildResolver AddChildResolver(Func<Type, bool> filter,
                                                     Func<DependencyObject, IEnumerable<DependencyObject>> resolver)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            NonResolvableChildTypes.Clear();

            var childResolver = new ChildResolver(filter, resolver);

            ChildResolvers.Add(childResolver);

            return childResolver;
        }

        /// <summary>
        ///     Adds a child resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public static ChildResolver AddChildResolver<T>(Func<T, IEnumerable<DependencyObject>> resolver)
            where T : DependencyObject
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            NonResolvableChildTypes.Clear();

            var childResolver = new ChildResolver<T>(resolver);

            ChildResolvers.Add(childResolver);

            return childResolver;
        }

        /// <summary>
        ///     Searches through the list of named elements looking for a case-insensitive match.
        /// </summary>
        /// <param name="elementsToSearch">The named elements to search through.</param>
        /// <param name="name">The name to search for.</param>
        /// <returns>The named element or null if not found.</returns>
        public static FrameworkElement FindName(this IEnumerable<FrameworkElement> elementsToSearch, string name)
        {
            return elementsToSearch.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Removes a child resolver.
        /// </summary>
        /// <param name="resolver">The resolver to remove.</param>
        /// <returns>true, when the resolver was (found and) removed.</returns>
        public static bool RemoveChildResolver(ChildResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return ChildResolvers.Remove(resolver);
        }

        /// <summary>
        ///     Maintains a connection in the visual tree of dependency objects in order to record a route through it.
        /// </summary>
        public class ScopeNamingRoute
        {
            private readonly Dictionary<DependencyObject, DependencyObject> _path =
                new Dictionary<DependencyObject, DependencyObject>();

            private DependencyObject _root;

            /// <summary>
            ///     Gets or sets the starting point of the route.
            /// </summary>
            public DependencyObject Root
            {
                get { return _root; }
                set
                {
                    if (_path.ContainsValue(value))
                    {
                        throw new ArgumentException("Value is a target of some route hop; cannot be a root.");
                    }

                    _root = value;
                }
            }

            /// <summary>
            ///     Adds a segment to the route.
            /// </summary>
            /// <param name="from">The source dependency object.</param>
            /// <param name="to">The target dependency object.</param>
            public void AddHop(DependencyObject from, DependencyObject to)
            {
                if (from == null)
                {
                    throw new ArgumentNullException(nameof(from));
                }

                if (to == null)
                {
                    throw new ArgumentNullException(nameof(to));
                }

                if (_path.Count > 0 &&
                    !_path.ContainsKey(from) &&
                    !_path.ContainsKey(to) &&
                    !_path.ContainsValue(from) &&
                    !_path.ContainsValue(from))
                {
                    throw new ArgumentException("Hop pair not part of existing route.");
                }

                if (_path.ContainsKey(to))
                {
                    throw new ArgumentException("Cycle detected when adding hop.");
                }

                _path[from] = to;
            }

            /// <summary>
            ///     Tries to get a target dependency object given a source.
            /// </summary>
            /// <param name="hopSource">The possible beginning of a route segment (hop).</param>
            /// <param name="hopTarget">The target of a route segment (hop).</param>
            /// <returns>
            ///     <see langword="true" /> if <paramref name="hopSource" /> had a target recorded; <see langword="false" />
            ///     otherwise.
            /// </returns>
            public bool TryGetHop(DependencyObject hopSource, out DependencyObject hopTarget)
            {
                return _path.TryGetValue(hopSource, out hopTarget);
            }
        }
    }
}