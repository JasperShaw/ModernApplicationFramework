using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class Selection : DisplayTextRange
    {
        public abstract void SelectRange(TextRange textRange);

        public abstract void SelectRange(TextPoint selectionStart, TextPoint selectionEnd);

        public abstract void SelectAll();

        public abstract void ExtendSelection(TextPoint newEnd);

        public abstract void Clear();

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextSelection AdvancedSelection { get; }

        public abstract bool IsReversed { get; set; }
    }
}