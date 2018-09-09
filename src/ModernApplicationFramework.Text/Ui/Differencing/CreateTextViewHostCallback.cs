using System.Windows;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Differencing
{
    public delegate void CreateTextViewHostCallback(IDifferenceTextViewModel textViewModel, ITextViewRoleSet roles,
        IEditorOptions options, out FrameworkElement visualElement, out ITextViewHost textViewHost);
}