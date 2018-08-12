using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class AdornmentLayer : Canvas, IAdornmentLayer
    {
        internal List<AdornmentAndData> _elements;
        private readonly bool _isOverlayLayer;

        public AdornmentLayer(ITextView view, bool isOverlayLayer = false)
        {
            TextView = view;
            _elements = new List<AdornmentAndData>();
            _isOverlayLayer = isOverlayLayer;
        }

        public static bool IsTextRelative(AdornmentPositioningBehavior behavior)
        {
            if (behavior != AdornmentPositioningBehavior.TextRelative)
                return behavior == (AdornmentPositioningBehavior.ViewportRelative |
                                    AdornmentPositioningBehavior.TextRelative);
            return true;
        }

        public bool AddAdornment(SnapshotSpan visualSpan, object tag, UIElement element)
        {
            return AddAdornment(AdornmentPositioningBehavior.TextRelative, visualSpan, tag, element, null);
        }

        public bool AddAdornment(AdornmentPositioningBehavior behavior, SnapshotSpan? visualSpan, object tag,
            UIElement element, AdornmentRemovedCallback removedCallback)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (_isOverlayLayer && behavior != AdornmentPositioningBehavior.OwnerControlled)
                throw new ArgumentOutOfRangeException(nameof(behavior),
                    "Only AdornmentPositioningBehavior.OwnerControlled is supported");
            if (IsTextRelative(behavior) && !visualSpan.HasValue)
                throw new ArgumentNullException(nameof(visualSpan));
            var flag = true;
            if (visualSpan.HasValue)
                flag = TextView.TextViewLines.IntersectsBufferSpan(visualSpan.Value);
            if (flag)
            {
                _elements.Add(new AdornmentAndData(behavior, visualSpan, tag, element, removedCallback));
                Children.Add(element);
            }

            return flag;
        }

        public void RemoveAllAdornments()
        {
            foreach (var element in _elements)
                RemoveTranslatableVisual(element);

            _elements = new List<AdornmentAndData>();
        }

        public void RemoveAdornment(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            for (var index = 0; index < _elements.Count; ++index)
            {
                var element1 = _elements[index];
                if (element1.Adornment == element)
                {
                    _elements.RemoveAt(index);
                    RemoveTranslatableVisual(element1);
                    break;
                }
            }
        }

        public void RemoveAdornmentsByVisualSpan(SnapshotSpan visualSpan)
        {
            InternalRemoveMatchingAdornments(visualSpan, AlwaysTrue);
        }

        public void RemoveAdornmentsByTag(object tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            InternalRemoveMatchingAdornments(new SnapshotSpan?(), adornment => Equals(adornment.Tag, tag));
        }

        public void RemoveMatchingAdornments(Predicate<IAdornmentLayerElement> match)
        {
            InternalRemoveMatchingAdornments(new SnapshotSpan?(), match);
        }

        public void RemoveMatchingAdornments(SnapshotSpan visualSpan, Predicate<IAdornmentLayerElement> match)
        {
            InternalRemoveMatchingAdornments(visualSpan, match);
        }

        public ITextView TextView { get; }

        public bool IsEmpty => _elements.Count == 0;

        public new double Opacity
        {
            get => base.Opacity;
            set => base.Opacity = value;
        }

        public ReadOnlyCollection<IAdornmentLayerElement> Elements
        {
            get
            {
                return new ReadOnlyCollection<IAdornmentLayerElement>(_elements.ConvertAll(d =>
                    (IAdornmentLayerElement) d));
            }
        }

        private static bool AlwaysTrue(IAdornmentLayerElement adornment)
        {
            return true;
        }

        private void InternalRemoveMatchingAdornments(SnapshotSpan? visualSpan, Predicate<IAdornmentLayerElement> match)
        {
            var adornmentAndDataList = new List<AdornmentAndData>(_elements.Count);
            foreach (var element in _elements)
            {
                if ((!visualSpan.HasValue || element.OverlapsWith(visualSpan.Value)) && match(element))
                    RemoveTranslatableVisual(element);
                else
                    adornmentAndDataList.Add(element);
            }

            _elements = adornmentAndDataList;
        }

        internal void SetSnapshotAndUpdate(ITextSnapshot snapshot, double deltaX, double deltaY,
            IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
        {
            var adornmentAndDataList = new List<AdornmentAndData>(_elements.Count);
            foreach (var element in _elements)
            {
                var visualSpan = element.VisualSpan;
                if (!visualSpan.HasValue)
                {
                    adornmentAndDataList.Add(element);
                    if (element.Behavior == AdornmentPositioningBehavior.ViewportRelative)
                        element.Translate(deltaX, deltaY);
                }
                else
                {
                    element.SetSnapshot(snapshot);
                    visualSpan = element.VisualSpan;
                    var snapshotSpan = visualSpan.Value;
                    if (!TextView.TextViewLines.IntersectsBufferSpan(snapshotSpan) ||
                        GetCrossingLine(newOrReformattedLines, snapshotSpan) != null)
                    {
                        RemoveTranslatableVisual(element);
                    }
                    else
                    {
                        adornmentAndDataList.Add(element);
                        var behavior = element.Behavior;
                        if (behavior != AdornmentPositioningBehavior.ViewportRelative)
                        {
                            if ((uint) (behavior - 2) > 1U)
                                continue;
                            var crossingLine = GetCrossingLine(translatedLines, snapshotSpan);
                            if (crossingLine != null)
                                element.Translate(
                                    element.Behavior == AdornmentPositioningBehavior.TextRelative ? 0.0 : deltaX,
                                    crossingLine.DeltaY);
                            else if (element.Behavior ==
                                     (AdornmentPositioningBehavior.ViewportRelative |
                                      AdornmentPositioningBehavior.TextRelative))
                                element.Translate(deltaX, 0.0);
                        }
                        else
                            element.Translate(deltaX, deltaY);
                    }
                }
            }

            _elements = adornmentAndDataList;
        }

        private void RemoveTranslatableVisual(IAdornmentLayerElement data)
        {
            Children.Remove(data.Adornment);
            data.RemovedCallback?.Invoke(data.Tag, data.Adornment);
        }

        internal static ITextViewLine GetCrossingLine(IList<ITextViewLine> lines, SnapshotSpan span)
        {
            if (lines.Count > 0)
            {
                int start = span.Start;
                var num1 = start + Math.Max(1, span.Length);
                var num2 = 0;
                var num3 = lines.Count;
                while (num2 < num3)
                {
                    var index = (num2 + num3) / 2;
                    var line = lines[index];
                    if (num1 <= line.Start)
                    {
                        num3 = index;
                    }
                    else
                    {
                        if (start < line.EndIncludingLineBreak)
                            return line;
                        num2 = index + 1;
                    }
                }

                if (num1 > span.Snapshot.Length)
                {
                    var line = lines[lines.Count - 1];
                    if (line.End == start)
                        return line;
                }
            }

            return null;
        }

        internal class AdornmentAndData : IAdornmentLayerElement
        {
            internal readonly UIElement Element;

            public AdornmentAndData(AdornmentPositioningBehavior behavior, SnapshotSpan? visualSpan, object tag,
                UIElement element, AdornmentRemovedCallback removedCallback)
            {
                Behavior = behavior;
                VisualSpan = visualSpan;
                Tag = tag;
                Element = element;
                RemovedCallback = removedCallback;
            }

            public AdornmentPositioningBehavior Behavior { get; }

            public UIElement Adornment => Element;

            public AdornmentRemovedCallback RemovedCallback { get; }

            public object Tag { get; }

            public SnapshotSpan? VisualSpan { get; private set; }

            public bool OverlapsWith(SnapshotSpan visualSpan)
            {
                if (!VisualSpan.HasValue)
                    return false;
                var snapshotSpan = VisualSpan.Value;
                int start = snapshotSpan.Start;
                snapshotSpan = VisualSpan.Value;
                var length1 = snapshotSpan.Length;
                var length2 = Math.Max(1, length1);
                var local = new Span(start, length2);
                return visualSpan.Start < local.End && (visualSpan.End > local.Start ||
                                                       visualSpan.End == local.Start &&
                                                       local.Start == visualSpan.Snapshot.Length);
            }

            public void SetSnapshot(ITextSnapshot snapshot)
            {
                VisualSpan = VisualSpan.Value.TranslateTo(snapshot, SpanTrackingMode.EdgeInclusive);
            }

            public void Translate(double deltaX, double deltaY)
            {
                if (deltaX == 0.0 && deltaY == 0.0)
                    return;
                var d1 = GetLeft(Element);
                if (double.IsNaN(d1))
                    d1 = 0.0;
                var d2 = GetTop(Element);
                if (double.IsNaN(d2))
                    d2 = 0.0;
                SetLeft(Element, d1 + deltaX);
                SetTop(Element, d2 + deltaY);
            }
        }
    }
}