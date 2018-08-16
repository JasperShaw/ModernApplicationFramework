using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Text
{
    public abstract class AbstractSelectionPresentationProperties
    {
        public virtual double PreferredXCoordinate { get; protected set; }

        public virtual double PreferredYCoordinate { get; protected set; }

        public virtual TextBounds CaretBounds { get; }

        public virtual bool IsWithinViewport { get; }

        public virtual bool IsOverwriteMode { get; }

        public virtual ITextViewLine ContainingTextViewLine { get; }
    }
}