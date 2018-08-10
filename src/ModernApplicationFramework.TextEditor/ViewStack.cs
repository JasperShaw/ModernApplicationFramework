using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.TextEditor
{
    internal class ViewStack : Canvas
    {
        internal IList<UiElementData> _elements = new List<UiElementData>();
        private readonly Dictionary<string, int> _orderedViewLayerDefinitions;
        private readonly ITextView _textView;
        private readonly bool _isOverlayLayer;

        public ViewStack(Dictionary<string, int> orderedViewLayerDefinitions, ITextView textView, bool isOverlayLayer = false)
        {
            _orderedViewLayerDefinitions = orderedViewLayerDefinitions;
            _textView = textView;
            _isOverlayLayer = isOverlayLayer;
        }

        public bool TryAddElement(string name, UIElement element)
        {
            if (!_orderedViewLayerDefinitions.TryGetValue(name, out var rank))
                return false;
            var data = new UiElementData(element, name, rank);
            var index = 0;
            while (index < _elements.Count && _elements[index].Rank <= rank)
                ++index;
            Children.Insert(index, element);
            if (element is FrameworkElement fe)
            {
                fe.Width = ActualWidth;
                fe.Height = ActualHeight;
            }
            _elements.Insert(index, data);
            return true;
        }

        public void SetSnapshotAndUpdate(ITextSnapshot snapshot, double deltaX, double deltaY, IList<ITextViewLine> newOrReformattedLines, IList<ITextViewLine> translatedLines)
        {
            foreach (var element in _elements)
                (element.Element as AdornmentLayer)?.SetSnapshotAndUpdate(snapshot, deltaX, deltaY,
                    newOrReformattedLines, translatedLines);
        }

        public UIElement GetElement(string name)
        {
            return (from element in _elements where element.Name == name select element.Element).FirstOrDefault();
        }

        public void SetSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
            if (_isOverlayLayer)
                ClipToBounds = true;
            else
                VisualScrollableAreaClip = new Rect(new Point(0, 0), size);
        }

        internal class UiElementData
        {
            public readonly UIElement Element;
            public readonly string Name;
            public readonly int Rank;

            public UiElementData(UIElement element, string name, int rank)
            {
                Element = element;
                Name = name;
                Rank = rank;
            }
        }
    }
}