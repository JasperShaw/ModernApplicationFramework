using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Utilities
{
    public sealed class TrackingSpanTree<T>
    {
        private int _advanceVersion;

        public ITextBuffer Buffer { get; }

        public int Count { get; private set; }

        public TrackingSpanNode<T> Root { get; }

        public TrackingSpanTree(ITextBuffer buffer, bool keepTrackingCurrent)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
            Count = 0;
            Root = new TrackingSpanNode<T>(default, null);
            if (!keepTrackingCurrent)
                return;
            buffer.Changed += OnBufferChanged;
        }

        private enum FindResultType
        {
            Inner,
            Outer
        }

        public void Advance(ITextVersion toVersion)
        {
            if (toVersion == null)
                throw new ArgumentNullException(nameof(toVersion));
            if (toVersion.VersionNumber <= _advanceVersion)
                return;
            _advanceVersion = toVersion.VersionNumber;
            Root.Advance(toVersion);
        }

        public void Clear()
        {
            Root.Children.Clear();
            Count = 0;
        }

        public IEnumerable<TrackingSpanNode<T>> FindNodesContainedBy(SnapshotSpan span)
        {
            return FindNodesContainedBy(new NormalizedSnapshotSpanCollection(span));
        }

        public IEnumerable<TrackingSpanNode<T>> FindNodesContainedBy(NormalizedSnapshotSpanCollection spans)
        {
            return FindNodes(spans, Root, true, true);
        }

        public IEnumerable<TrackingSpanNode<T>> FindNodesIntersecting(SnapshotSpan span)
        {
            return FindNodesIntersecting(new NormalizedSnapshotSpanCollection(span));
        }

        public IEnumerable<TrackingSpanNode<T>> FindNodesIntersecting(NormalizedSnapshotSpanCollection spans)
        {
            return FindNodes(spans, Root);
        }

        public IEnumerable<TrackingSpanNode<T>> FindTopLevelNodesContainedBy(SnapshotSpan span)
        {
            return FindTopLevelNodesContainedBy(new NormalizedSnapshotSpanCollection(span));
        }

        public IEnumerable<TrackingSpanNode<T>> FindTopLevelNodesContainedBy(NormalizedSnapshotSpanCollection spans)
        {
            return FindNodes(spans, Root, false, true);
        }

        public IEnumerable<TrackingSpanNode<T>> FindTopLevelNodesIntersecting(SnapshotSpan span)
        {
            return FindTopLevelNodesIntersecting(new NormalizedSnapshotSpanCollection(span));
        }

        public IEnumerable<TrackingSpanNode<T>> FindTopLevelNodesIntersecting(NormalizedSnapshotSpanCollection spans)
        {
            return FindNodes(spans, Root, false);
        }

        public bool IsNodeTopLevel(TrackingSpanNode<T> node)
        {
            return Root.Children.Contains(node);
        }

        public bool IsPointContainedInANode(SnapshotPoint point)
        {
            return FindChild(point, Root.Children, true).Type == FindResultType.Inner;
        }

        public bool RemoveItem(T item, ITrackingSpan trackingSpan)
        {
            if (trackingSpan == null)
                throw new ArgumentNullException(nameof(trackingSpan));
            var span = trackingSpan.GetSpan(Buffer.CurrentSnapshot);
            if (!RemoveItemFromRoot(item, span, Root))
                return false;
            --Count;
            return true;
        }

        public TrackingSpanNode<T> TryAddItem(T item, ITrackingSpan trackingSpan)
        {
            if (trackingSpan == null)
                throw new ArgumentNullException(nameof(trackingSpan));
            if (trackingSpan.TrackingMode != SpanTrackingMode.EdgeExclusive)
                throw new ArgumentException(
                    "The tracking mode of the given tracking span must be SpanTrackingMode.EdgeExclusive",
                    nameof(trackingSpan));
            var span = trackingSpan.GetSpan(Buffer.CurrentSnapshot);
            var root = TryAddNodeToRoot(new TrackingSpanNode<T>(item, trackingSpan), span, Root);
            if (root == null)
                return root;
            ++Count;
            return root;
        }

        private static FindResult FindChild(SnapshotPoint point, List<TrackingSpanNode<T>> nodes, bool left,
            int lo = -1, int hi = -1)
        {
            if (nodes.Count == 0)
                return new FindResult
                {
                    Index = 0,
                    Intersects = false,
                    Type = FindResultType.Outer
                };
            var snapshot = point.Snapshot;
            var position = point.Position;
            var findResultType = FindResultType.Outer;
            var flag = false;
            if (lo == -1)
                lo = 0;
            if (hi == -1)
                hi = nodes.Count - 1;
            var index1 = lo;
            while (lo <= hi)
            {
                index1 = (lo + hi) / 2;
                var span = nodes[index1].TrackingSpan.GetSpan(snapshot);
                if (position < span.Start)
                {
                    hi = index1 - 1;
                }
                else if (position > span.End)
                {
                    lo = index1 + 1;
                }
                else
                {
                    if (position > span.Start && position < span.End)
                        findResultType = FindResultType.Inner;
                    flag = true;
                    break;
                }
            }

            var index2 = index1;
            nodes[index2].TrackingSpan.GetSpan(snapshot);
            if (flag)
            {
                if (left)
                {
                    for (; index2 >= lo; --index2)
                    {
                        var span = nodes[index2].TrackingSpan.GetSpan(snapshot);
                        if (position > span.End)
                        {
                            ++index2;
                            break;
                        }
                    }

                    if (index2 < lo)
                        index2 = lo;
                }
                else
                {
                    for (; index2 <= hi; ++index2)
                    {
                        var span = nodes[index2].TrackingSpan.GetSpan(snapshot);
                        if (position < span.Start)
                        {
                            --index2;
                            break;
                        }
                    }

                    if (index2 > hi)
                        index2 = hi;
                }
            }

            return new FindResult
            {
                Type = findResultType,
                Index = index2,
                Intersects = flag
            };
        }

        private static FindResult FindIndexForAdd(SnapshotPoint point, List<TrackingSpanNode<T>> nodes, bool left,
            int lo = -1, int hi = -1)
        {
            var snapshot = point.Snapshot;
            var position = point.Position;
            if (lo == -1)
                lo = 0;
            if (hi == -1)
                hi = nodes.Count - 1;
            var child = FindChild(point, nodes, left, lo, hi);
            var index = child.Index;
            var span1 = nodes[index].TrackingSpan.GetSpan(snapshot);
            if (!child.Intersects)
            {
                if (position < span1.Start && !left)
                    --index;
                else if ((position > span1.End) & left)
                    ++index;
            }
            else if (child.Type == FindResultType.Outer)
            {
                if (left)
                    for (; index <= hi; ++index)
                    {
                        var span2 = nodes[index].TrackingSpan.GetSpan(snapshot);
                        if (position <= span2.Start)
                            break;
                    }
                else
                    for (; index >= lo; --index)
                    {
                        var span2 = nodes[index].TrackingSpan.GetSpan(snapshot);
                        if (position >= span2.End)
                            break;
                    }
            }

            return new FindResult
            {
                Type = child.Type,
                Index = index,
                Intersects = child.Intersects
            };
        }

        private static int FindNextChild(TrackingSpanNode<T> root, SnapshotPoint point, int currentChildIndex)
        {
            if (currentChildIndex == root.Children.Count - 1)
                return currentChildIndex + 1;
            return FindChild(point, root.Children, true, currentChildIndex + 1).Index;
        }

        private static IEnumerable<TrackingSpanNode<T>> FindNodes(NormalizedSnapshotSpanCollection spans,
            TrackingSpanNode<T> root, bool recurse = true, bool contained = false)
        {
            if (!(spans == null) && spans.Count != 0 && root.Children.Count != 0)
            {
                var requestIndex = 0;
                var currentRequest = spans[requestIndex];
                var childIndex = FindChild(currentRequest.Start, root.Children, true).Index;
                if (childIndex < root.Children.Count)
                {
                    var snapshot = currentRequest.Snapshot;
                    var currentChild = root.Children[childIndex].TrackingSpan.GetSpan(snapshot);
                    while (requestIndex < spans.Count && childIndex < root.Children.Count)
                        if (currentRequest.Start > currentChild.End)
                        {
                            childIndex = FindNextChild(root, currentRequest.Start, childIndex);
                            if (childIndex < root.Children.Count)
                                currentChild = root.Children[childIndex].TrackingSpan.GetSpan(snapshot);
                        }
                        else if (currentChild.Start > currentRequest.End)
                        {
                            if (++requestIndex < spans.Count)
                                currentRequest = spans[requestIndex];
                        }
                        else
                        {
                            if (!contained || currentRequest.Contains(currentChild))
                                yield return root.Children[childIndex];
                            if (recurse)
                                foreach (var node in FindNodes(spans, root.Children[childIndex], recurse, contained))
                                    yield return node;
                            childIndex = FindNextChild(root, currentRequest.Start, childIndex);
                            if (childIndex < root.Children.Count)
                                currentChild = root.Children[childIndex].TrackingSpan.GetSpan(snapshot);
                        }
                }
            }
        }

        private static bool RemoveItemFromRoot(T item, SnapshotSpan span, TrackingSpanNode<T> root)
        {
            if (root.Children.Count == 0)
                return false;
            var child1 = FindChild(span.Start, root.Children, true);
            if (child1.Index < 0 || child1.Index >= root.Children.Count)
                return false;
            for (var index = child1.Index; index < root.Children.Count; ++index)
            {
                var child2 = root.Children[index];
                var span1 = child2.TrackingSpan.GetSpan(span.Snapshot);
                if (span1.Start > span.End)
                    return false;
                if (span1 == span && Equals(child2.Item, item))
                {
                    root.Children.RemoveAt(index);
                    root.Children.InsertRange(index, child2.Children);
                    return true;
                }

                if (span1.Contains(span) && RemoveItemFromRoot(item, span, child2))
                    return true;
            }

            return false;
        }

        private static TrackingSpanNode<T> TryAddNodeToRoot(TrackingSpanNode<T> newNode, SnapshotSpan span,
            TrackingSpanNode<T> root)
        {
            var children = root.Children;
            if (children.Count == 0)
            {
                children.Add(newNode);
                return newNode;
            }

            var indexForAdd1 = FindIndexForAdd(span.Start, children, true);
            var indexForAdd2 = FindIndexForAdd(span.End, children, false);
            if (indexForAdd1.Index > indexForAdd2.Index)
            {
                children.Insert(indexForAdd1.Index, newNode);
                return newNode;
            }

            if (indexForAdd1.Type == FindResultType.Inner || indexForAdd2.Type == FindResultType.Inner)
            {
                if (children[indexForAdd1.Index].TrackingSpan.GetSpan(span.Snapshot).Contains(span))
                    return TryAddNodeToRoot(newNode, span, children[indexForAdd1.Index]);
                if (indexForAdd1.Index != indexForAdd2.Index &&
                    children[indexForAdd2.Index].TrackingSpan.GetSpan(span.Snapshot).Contains(span))
                    return TryAddNodeToRoot(newNode, span, children[indexForAdd2.Index]);
                return null;
            }

            var index = indexForAdd1.Index;
            var count = indexForAdd2.Index - indexForAdd1.Index + 1;
            newNode.Children.AddRange(children.Skip(index).Take(count));
            children.RemoveRange(index, count);
            children.Insert(index, newNode);
            return newNode;
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs args)
        {
            Advance(args.After.Version);
        }

        private struct FindResult
        {
            public int Index;
            public bool Intersects;
            public FindResultType Type;
        }
    }
}