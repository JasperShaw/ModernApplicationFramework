using System;
using System.Windows;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal sealed class OverviewScrollBarMargin : ITextViewMargin
    {
        public const string MarginName = "OverviewScrollBarMargin";
        internal readonly OverviewScrollBarMarginControl Margin;
        internal readonly OverviewScrollBarViewModel Model;
        private readonly ITextView _textView;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly OverviewElement _overviewElement;
        private bool _isDisposed;

        public OverviewScrollBarMargin(ITextViewHost textViewHost, OverviewElementFactory overviewElementFactory)
        {
            _textView = textViewHost.TextView;
            _editorFormatMap = overviewElementFactory.EditorFormatMapService.GetEditorFormatMap(textViewHost.TextView);
            _overviewElement = overviewElementFactory.CreateElement(textViewHost);
            Model = new OverviewScrollBarViewModel(() => _textView.ViewScroller.ScrollViewportVerticallyByLine(ScrollDirection.Up), () => _textView.ViewScroller.ScrollViewportVerticallyByLine(ScrollDirection.Down));
            var barMarginControl = new OverviewScrollBarMarginControl(_overviewElement) {DataContext = Model};
            Margin = barMarginControl;
            OnFormatMappingChanged(null, null);
            OnOptionsChanged(null, null);
            _textView.Options.OptionChanged += OnOptionsChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e != null && !e.ChangedItems.Contains("OverviewMarginScrollButtons") && !e.ChangedItems.Contains("OverviewMarginScrollButtonsMouseOver") && !e.ChangedItems.Contains("OverviewMarginScrollButtonsMouseDown"))
                return;
            Model.ScrollBarArrowBackground = _editorFormatMap.GetBrush("OverviewMarginScrollButtons", "Background");
            Model.ScrollBarArrowGlyph = _editorFormatMap.GetBrush("OverviewMarginScrollButtons", "Foreground");
            Model.ScrollBarArrowMouseOverBackground = _editorFormatMap.GetBrush("OverviewMarginScrollButtonsMouseOver", "Background");
            Model.ScrollBarArrowMouseOverGlyph = _editorFormatMap.GetBrush("OverviewMarginScrollButtonsMouseOver", "Foreground");
            Model.ScrollBarArrowPressedBackground = _editorFormatMap.GetBrush("OverviewMarginScrollButtonsMouseDown", "Background");
            Model.ScrollBarArrowPressedGlyph = _editorFormatMap.GetBrush("OverviewMarginScrollButtonsMouseDown", "Foreground");
        }

        public FrameworkElement VisualElement => Margin;

        public bool Enabled => _textView.Options.GetOptionValue(DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionId);

        public double MarginSize => VisualElement.ActualWidth;

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _textView.Options.OptionChanged -= OnOptionsChanged;
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _overviewElement.Dispose();
            _isDisposed = true;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (!string.Equals(marginName, nameof(OverviewScrollBarMargin), StringComparison.OrdinalIgnoreCase))
                return null;
            return this;
        }

        private void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e != null && !string.Equals(e.OptionId, "OverviewMargin/ShowEnhancedScrollBar", StringComparison.Ordinal))
                return;
            VisualElement.Visibility = Enabled ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
