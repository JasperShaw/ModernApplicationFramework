using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal static class StableTopologicalSort
    {
        public static IEnumerable<T> Order<T>(IEnumerable<T> itemsToOrder, TopologicalDependencyFunction<T> dependencyFunction)
        {
            if (itemsToOrder == null)
                throw new ArgumentNullException(nameof(itemsToOrder));
            if (dependencyFunction == null)
                throw new ArgumentNullException(nameof(dependencyFunction));
            var toOrder = itemsToOrder as T[] ?? itemsToOrder.ToArray();
            var list = toOrder.ToList();
            if (list.Count < 2)
                return toOrder;
            var count = list.Count;
            var dependencyGraph = DependencyGraph<T>.TryCreate(list, dependencyFunction, EqualityComparer<T>.Default);
            if (dependencyGraph == null)
                return toOrder;
            loop:
            for (var index1 = 0; index1 < count; ++index1)
            {
                for (var index2 = 0; index2 < index1; ++index2)
                {
                    if (dependencyGraph.DoesXHaveDirectDependencyOnY(list[index2], list[index1]) && !(dependencyGraph.DoesXHaveTransientDependencyOnY(list[index2], list[index1]) & dependencyGraph.DoesXHaveTransientDependencyOnY(list[index1], list[index2])))
                    {
                        var obj = list[index1];
                        list.RemoveAt(index1);
                        list.Insert(index2, obj);
                        goto loop;
                    }
                }
            }
            return list;
        }


        public delegate bool TopologicalDependencyFunction<in T>(T x, T y);

        private class DependencyGraph<T>
        {
            private IEqualityComparer<T> EqualityComparer { get; }

            private IDictionary<T, Node> Nodes { get; }

            private DependencyGraph(IEqualityComparer<T> equalityComparer, int n)
            {
                var equalityComparer1 = equalityComparer;
                EqualityComparer = equalityComparer1 ?? throw new ArgumentNullException(nameof(equalityComparer));
                Nodes = new Dictionary<T, Node>(n, equalityComparer);
            }

            public static DependencyGraph<T> TryCreate(ICollection<T> items, TopologicalDependencyFunction<T> dependencyFunction, IEqualityComparer<T> equalityComparer)
            {
                var dependencyGraph = new DependencyGraph<T>(equalityComparer, items.Count);
                var flag = false;
                foreach (var obj in items)
                {
                    if (!dependencyGraph.Nodes.TryGetValue(obj, out var node))
                    {
                        node = new Node();
                        dependencyGraph.Nodes.Add(obj, node);
                    }
                    foreach (var y in items)
                    {
                        if (!equalityComparer.Equals(obj, y) && dependencyFunction(obj, y))
                        {
                            node.Children.Add(y);
                            flag = true;
                        }
                    }
                }
                if (!flag)
                    return null;
                return dependencyGraph;
            }

            public bool DoesXHaveDirectDependencyOnY(T x, T y)
            {
                return Nodes.TryGetValue(x, out var node) && node.Children.Contains(y, EqualityComparer);
            }

            public bool DoesXHaveTransientDependencyOnY(T x, T y)
            {
                return new DependencyWalker(this).DoesXHaveTransientDependencyOnY(x, y);
            }

            private class Node
            {
                private IList<T> _children = new FrugalList<T>();

                public IList<T> Children => _children ?? (_children = new FrugalList<T>());
            }

            private class DependencyWalker
            {
                private readonly DependencyGraph<T> _graph;
                private readonly HashSet<T> _visitedNodes;

                public DependencyWalker(DependencyGraph<T> graph)
                {
                    _graph = graph;
                    _visitedNodes = new HashSet<T>(graph.EqualityComparer);
                }

                public bool DoesXHaveTransientDependencyOnY(T x, T y)
                {
                    if (!_visitedNodes.Add(x) || !_graph.Nodes.TryGetValue(x, out var node))
                        return false;
                    return node.Children.Contains(y, _graph.EqualityComparer) || node.Children.Any(child => DoesXHaveTransientDependencyOnY(child, y));
                }
            }
        }
    }
}