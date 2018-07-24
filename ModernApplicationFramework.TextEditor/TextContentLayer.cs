using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextContentLayer : FrameworkElement
    {
        private bool _removeTextVisualsWhenHidden = true;
        private bool _needsRefresh;
        private List<IFormattedLine> _formattedLines;
        private List<Visual> _children = new List<Visual>();


        public bool RemoveTextVisualsWhenHidden
        {
            get => _removeTextVisualsWhenHidden;
            set
            {
                if (value == _removeTextVisualsWhenHidden)
                    return;
                _removeTextVisualsWhenHidden = value;
                if (IsVisible)
                    return;
                if (_removeTextVisualsWhenHidden)
                    RemoveTextVisuals();
                else
                    UpdateTextVisuals();
            }
        }

        protected override int VisualChildrenCount => _children.Count;

        public TextContentLayer()
        {
            IsVisibleChanged += OnVisibleChanged;
            IsHitTestVisible = false;
            Name = "Maf_TextEditorContent";
        }

        public void SetTextViewLines(List<IFormattedLine> formattedLines)
        {
            _formattedLines = formattedLines;
            if ((!_removeTextVisualsWhenHidden ? 1 : (IsVisible ? 1 : 0)) == 0)
                return;
            InvalidateArrange();
            _needsRefresh = true;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_needsRefresh)
            {
                UpdateTextVisuals();
                _needsRefresh = false;
            }
            SignalName();
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > _children.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _children[index];
        }

        private void SignalName()
        {
            var name = Name;
            Name = "temporary_toggle_value";
            Name = name;
        }


        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((!_removeTextVisualsWhenHidden ? 1 : (IsVisible ? 1 : 0)) != 0)
            {
                InvalidateArrange();
                _needsRefresh = true;
            }
            else
                RemoveTextVisuals();
        }

        private void RemoveTextVisuals()
        {
            if (_formattedLines == null)
                return;
            foreach (var line in _formattedLines)
                line.RemoveVisual();
            foreach (var child in _children)
                RemoveVisualChild(child);
            _children.Clear();
        }

        private void UpdateTextVisuals()
        {
            if (_formattedLines == null)
                return;
            var visualList = new List<Visual>(_formattedLines.Count);
            var dict = new Dictionary<Visual, SettableBool>(_formattedLines.Count);
            foreach (var line in _formattedLines)
            {
                if (line.VisibilityState == VisibilityState.Hidden)
                    line.RemoveVisual();
                else
                {
                    var visual = line.GetOrCreateVisual();
                    visualList.Add(visual);
                    dict.Add(visual, new SettableBool());
                }
            }

            foreach (var child in _children)
            {
                if (dict.TryGetValue(child, out var settableBool))
                    settableBool.AddVisual = false;
                else
                    RemoveVisualChild(child);
            }

            foreach (var pair  in dict)
            {
                if (pair.Value.AddVisual)
                    AddVisualChild(pair.Key);
            }

            _children = visualList;
        }

        private class SettableBool
        {
            public bool AddVisual = true;
        }
    }
}