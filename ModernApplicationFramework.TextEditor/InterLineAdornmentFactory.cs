using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public delegate UIElement InterLineAdornmentFactory(InterLineAdornmentTag tag, ITextView view, SnapshotPoint position);
}