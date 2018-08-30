using System;
using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Utilities
{
    public static class Orderer
    {
        public static IList<Lazy<TValue, TMetadata>> Order<TValue, TMetadata>(IEnumerable<Lazy<TValue, TMetadata>> itemsToOrder) where TValue : class where TMetadata : IOrderable
        {
            if (itemsToOrder == null)
                throw new ArgumentNullException(nameof(itemsToOrder));
            var roots = new Queue<Node<TValue, TMetadata>>();
            var unsortedItems = new List<Node<TValue, TMetadata>>();
            PrepareGraph(itemsToOrder, roots, unsortedItems);
            return TopologicalSort(roots, unsortedItems);
        }

        private static void PrepareGraph<TValue, TMetadata>(IEnumerable<Lazy<TValue, TMetadata>> items, Queue<Node<TValue, TMetadata>> roots, List<Node<TValue, TMetadata>> unsortedItems) where TValue : class where TMetadata : IOrderable
        {
            var map = new Dictionary<string, Node<TValue, TMetadata>>();
            foreach (var lazy in items)
            {
                if (lazy != null && lazy.Metadata != null)
                {
                    var node = new Node<TValue, TMetadata>(lazy);
                    if (node.Name != string.Empty)
                    {
                        if (!map.ContainsKey(node.Name))
                        {
                            map.Add(node.Name, node);
                            unsortedItems.Add(node);
                        }
                    }
                    else
                        unsortedItems.Add(node);
                }
            }
            for (var index = unsortedItems.Count - 1; index >= 0; --index)
                unsortedItems[index].Resolve(map, unsortedItems);
            var newRoots = new List<Node<TValue, TMetadata>>();
            foreach (var unsortedItem in unsortedItems)
            {
                if (unsortedItem.After.Count == 0)
                    newRoots.Add(unsortedItem);
            }
            AddToRoots(roots, newRoots);
        }

        private static IList<Lazy<TValue, TMetadata>> TopologicalSort<TValue, TMetadata>(Queue<Node<TValue, TMetadata>> roots, List<Node<TValue, TMetadata>> unsortedItems) where TValue : class where TMetadata : IOrderable
        {
            var lazyList = new List<Lazy<TValue, TMetadata>>();
            while (unsortedItems.Count > 0)
            {
                var node = roots.Count == 0 ? BreakCircularReference(unsortedItems) : roots.Dequeue();
                if (node.Item != null)
                    lazyList.Add(node.Item);
                unsortedItems.Remove(node);
                node.ClearBefore(roots);
            }
            return lazyList;
        }

        private static void AddToRoots<TValue, TMetadata>(Queue<Node<TValue, TMetadata>> roots, List<Node<TValue, TMetadata>> newRoots) where TValue : class where TMetadata : IOrderable
        {
            newRoots.Sort((l, r) => l.Name.CompareTo(r.Name));
            foreach (var newRoot in newRoots)
                roots.Enqueue(newRoot);
        }

        private static Node<TValue, TMetadata> BreakCircularReference<TValue, TMetadata>(List<Node<TValue, TMetadata>> unsortedItems) where TValue : class where TMetadata : IOrderable
        {
            var cycles = FindCycles(unsortedItems);
            var num1 = int.MaxValue;
            List<Node<TValue, TMetadata>> nodeList1 = null;
            foreach (var nodeList2 in cycles)
            {
                var num2 = 0;
                foreach (var node1 in nodeList2)
                {
                    foreach (var node2 in node1.After)
                    {
                        if (node2.LowIndex != node1.LowIndex)
                        {
                            ++num2;
                            break;
                        }
                    }
                }
                if (num2 < num1)
                {
                    nodeList1 = nodeList2;
                    num1 = num2;
                }
            }
            Node<TValue, TMetadata> node3;
            if (nodeList1 == null)
            {
                node3 = unsortedItems[0];
            }
            else
            {
                node3 = nodeList1[0];
                for (var index = 1; index < nodeList1.Count; ++index)
                {
                    var node1 = nodeList1[index];
                    if (node1.After.Count < node3.After.Count)
                        node3 = node1;
                }
            }
            foreach (var node1 in node3.After)
                node1.Before.Remove(node3);
            node3.After.Clear();
            return node3;
        }

        private static List<List<Node<TValue, TMetadata>>> FindCycles<TValue, TMetadata>(List<Node<TValue, TMetadata>> unsortedItems) where TValue : class where TMetadata : IOrderable
        {
            foreach (var unsortedItem in unsortedItems)
            {
                unsortedItem.Index = -1;
                unsortedItem.LowIndex = -1;
                unsortedItem.ContainedInKnownCycle = false;
            }
            var cycles = new List<List<Node<TValue, TMetadata>>>();
            var stack = new Stack<Node<TValue, TMetadata>>(unsortedItems.Count);
            var index = 0;
            foreach (var unsortedItem in unsortedItems)
            {
                if (unsortedItem.Index == -1)
                    FindCycles(unsortedItem, stack, ref index, cycles);
            }
            return cycles;
        }

        private static void FindCycles<TValue, TMetadata>(Node<TValue, TMetadata> node, Stack<Node<TValue, TMetadata>> stack, ref int index, List<List<Node<TValue, TMetadata>>> cycles) where TValue : class where TMetadata : IOrderable
        {
            node.Index = index;
            node.LowIndex = index;
            ++index;
            stack.Push(node);
            foreach (var node1 in node.Before)
            {
                if (node1.Index == -1)
                {
                    FindCycles(node1, stack, ref index, cycles);
                    node.LowIndex = Math.Min(node.LowIndex, node1.LowIndex);
                }
                else if (!node1.ContainedInKnownCycle)
                    node.LowIndex = Math.Min(node.LowIndex, node1.Index);
            }
            if (node.Index != node.LowIndex)
                return;
            var nodeList = new List<Node<TValue, TMetadata>>();
            while (stack.Count > 0)
            {
                var node1 = stack.Pop();
                nodeList.Add(node1);
                node1.ContainedInKnownCycle = true;
                if (node1 == node)
                {
                    if (nodeList.Count <= 1)
                        break;
                    cycles.Add(nodeList);
                    break;
                }
            }
        }

        private class Node<TValue, TMetadata> where TValue : class where TMetadata : IOrderable
        {
            public int Index = -1;
            public int LowIndex = -1;
            public readonly string Name;
            public readonly Lazy<TValue, TMetadata> Item;
            public bool ContainedInKnownCycle;

            public HashSet<Node<TValue, TMetadata>> After { get; } = new HashSet<Node<TValue, TMetadata>>();

            public HashSet<Node<TValue, TMetadata>> Before { get; } = new HashSet<Node<TValue, TMetadata>>();

            public Node(Lazy<TValue, TMetadata> item)
            {
                var name = item.Metadata.Name;
                Name = string.IsNullOrEmpty(name) ? string.Empty : name.ToUpperInvariant();
                Item = item;
            }

            private Node(string name)
            {
                Name = name;
            }

            public void Resolve(Dictionary<string, Node<TValue, TMetadata>> map, List<Node<TValue, TMetadata>> unsortedItems)
            {
                Resolve(map, Item.Metadata.After, After, unsortedItems);
                Resolve(map, Item.Metadata.Before, Before, unsortedItems);
                foreach (var node in Before)
                    node.After.Add(this);
                foreach (var node in After)
                    node.Before.Add(this);
            }

            public void ClearBefore(Queue<Node<TValue, TMetadata>> roots)
            {
                var newRoots = new List<Node<TValue, TMetadata>>();
                foreach (var node in Before)
                {
                    node.After.Remove(this);
                    if (node.After.Count == 0)
                        newRoots.Add(node);
                }
                Before.Clear();
                AddToRoots(roots, newRoots);
            }

            public override string ToString()
            {
                return Name;
            }

            private void Resolve(Dictionary<string, Node<TValue, TMetadata>> map, IEnumerable<string> links, HashSet<Node<TValue, TMetadata>> results, List<Node<TValue, TMetadata>> unsortedItems)
            {
                if (links == null)
                    return;
                foreach (var link in links)
                {
                    if (string.IsNullOrEmpty(link))
                        continue;
                    var upperInvariant = link.ToUpperInvariant();
                    if (!map.TryGetValue(upperInvariant, out var node))
                    {
                        node = new Node<TValue, TMetadata>(upperInvariant);
                        map.Add(upperInvariant, node);
                        unsortedItems.Add(node);
                    }
                    if (node != this)
                        results.Add(node);
                }
            }
        }
    }
}
