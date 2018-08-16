using System;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.Implementation;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class SelectionAdornmentLayer
    {
        private readonly ITextView _textView;
        private readonly IMultiSelectionBroker _multiSelectionBroker;
        internal readonly IAdornmentLayer AdornmentLayer;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        internal BrushSelectionPainter FocusedPainter;
        internal BrushSelectionPainter UnfocusedPainter;
        private MultiSelectionMouseState _mouseState;
        private BrushSelectionPainter _painter;

        public SelectionAdornmentLayer(MultiSelectionAdornmentProvider contextProvider, ITextView textView)
        {
            _textView = textView;
            _multiSelectionBroker = _textView.GetMultiSelectionBroker();
            _editorFormatMap = contextProvider.EditorFormatMapService.GetEditorFormatMap(textView);
            AdornmentLayer = textView.GetAdornmentLayer("SelectionAndProvisionHighlight");
            _classificationFormatMap = contextProvider.ClassificationFormatMappingService.GetClassificationFormatMap(textView);
            FocusedPainter = BrushSelectionPainter.CreatePainter(_multiSelectionBroker, AdornmentLayer, _editorFormatMap.GetProperties("Selected Text"), SystemColors.HighlightColor);
            UnfocusedPainter = BrushSelectionPainter.CreatePainter(_multiSelectionBroker, AdornmentLayer, _editorFormatMap.GetProperties("Inactive Selected Text"), SystemColors.GrayTextColor);
            _textView.LayoutChanged += OnTextViewLayoutChanged;
            _multiSelectionBroker.MultiSelectionSessionChanged += OnMultiSelectionSessionChanged;
            _editorFormatMap.FormatMappingChanged += OnFormatMappingChanged;
            _classificationFormatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
            _textView.Closed += OnTextViewClosed;
            _textView.Options.OptionChanged += OnEditorOptionChanged;
            MouseState.ProvisionalSelectionChanged += OnProvisionalSelectionChanged;
            _textView.GotAggregateFocus += OnGotAggregateFocus;
            _textView.LostAggregateFocus += OnLostAggregateFocus;
            Painter = FocusedPainter;
            Painter.Activate();
        }

        internal BrushSelectionPainter Painter
        {
            get => _painter;
            set
            {
                if (_painter == value)
                    return;
                _painter = value;
            }
        }

        private MultiSelectionMouseState MouseState => _mouseState ?? (_mouseState = MultiSelectionMouseState.GetStateForView(_textView));

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _editorFormatMap.FormatMappingChanged -= OnFormatMappingChanged;
            _classificationFormatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
            _multiSelectionBroker.MultiSelectionSessionChanged -= OnMultiSelectionSessionChanged;
            _textView.Options.OptionChanged -= OnEditorOptionChanged;
            MouseState.ProvisionalSelectionChanged -= OnProvisionalSelectionChanged;
        }

        private void OnFormatMappingChanged(object sender, FormatItemsEventArgs e)
        {
            if (e.ChangedItems.Contains("Selected Text"))
            {
                CreateAndSetPainter("Selected Text", ref FocusedPainter, SystemColors.HighlightColor);
                if (_multiSelectionBroker.AreSelectionsActive)
                    FocusedPainter.Activate();
            }
            if (!e.ChangedItems.Contains("Inactive Selected Text"))
                return;
            UnfocusedPainter = BrushSelectionPainter.CreatePainter(_multiSelectionBroker, AdornmentLayer, _editorFormatMap.GetProperties("Inactive Selected Text"), SystemColors.GrayTextColor);
            if (_multiSelectionBroker.AreSelectionsActive)
                return;
            UnfocusedPainter.Activate();
        }

        private void CreateAndSetPainter(string category, ref BrushSelectionPainter painter, Color defaultColor)
        {
            painter = BrushSelectionPainter.CreatePainter(_multiSelectionBroker, AdornmentLayer, _editorFormatMap.GetProperties(category), defaultColor);
        }

        private void OnEditorOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (string.CompareOrdinal(e.OptionId, DefaultViewOptions.EnableSimpleGraphicsId.Name) != 0)
                return;
            CreateAndSetPainter("Selected Text", ref FocusedPainter, SystemColors.HighlightColor);
            CreateAndSetPainter("Inactive Selected Text", ref UnfocusedPainter, SystemColors.GrayTextColor);
            Painter.Activate();
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
        }

        private void OnMultiSelectionSessionChanged(object sender, EventArgs e)
        {
            Redraw(true);
        }

        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            Redraw(false);
        }

        private void OnProvisionalSelectionChanged(object sender, EventArgs e)
        {
            Redraw(true);
        }

        private void OnLostAggregateFocus(object sender, EventArgs e)
        {
            Painter = _multiSelectionBroker.ActivationTracksFocus ? UnfocusedPainter : FocusedPainter;
            Redraw(false);
        }

        private void OnGotAggregateFocus(object sender, EventArgs e)
        {
            Painter = FocusedPainter;
            Redraw(false);
        }

        private void Redraw(bool selectionsChanged)
        {
            Painter.Update(selectionsChanged);
        }
    }
}
