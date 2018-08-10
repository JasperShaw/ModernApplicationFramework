using System.Windows;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public delegate UIElement InterLineAdornmentFactory(InterLineAdornmentTag tag, ITextView view, SnapshotPoint position);
}