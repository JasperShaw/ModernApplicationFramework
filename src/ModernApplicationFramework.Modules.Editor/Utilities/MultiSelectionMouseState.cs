using System;
using ModernApplicationFramework.Modules.Editor.MultiSelection;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public class MultiSelectionMouseState
    {
        private Selection _provisionalSelection = Selection.Invalid;
        private readonly ITextView _textView;

        public static MultiSelectionMouseState GetStateForView(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(
                () => new MultiSelectionMouseState(textView));
        }

        private MultiSelectionMouseState(ITextView textView)
        {
            _textView = textView;
            textView.LayoutChanged += OnLayoutChanged;
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (!(_provisionalSelection != Selection.Invalid))
                return;
            _provisionalSelection = _provisionalSelection.MapToSnapshot(e.NewSnapshot, _textView);
        }

        public Selection ProvisionalSelection
        {
            get => _provisionalSelection;
            set
            {
                if (!(_provisionalSelection != value))
                    return;
                _provisionalSelection = value;
                FireProvisionalSelectionChanged();
            }
        }

        public event EventHandler ProvisionalSelectionChanged;

        private void FireProvisionalSelectionChanged()
        {
            EventHandler selectionChanged = ProvisionalSelectionChanged;
            selectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool UserIsDraggingSelection { get; set; }
    }
}
