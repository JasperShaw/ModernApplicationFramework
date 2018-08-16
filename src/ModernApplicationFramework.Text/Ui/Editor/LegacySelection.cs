using System.ComponentModel;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class LegacySelection : DisplayTextRange
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextSelection AdvancedSelection { get; }

        public abstract bool IsReversed { get; set; }

        public abstract void Clear();

        public abstract void ExtendSelection(TextPoint newEnd);

        public abstract void SelectAll();
        public abstract void SelectRange(TextRange textRange);

        public abstract void SelectRange(TextPoint selectionStart, TextPoint selectionEnd);
    }
}