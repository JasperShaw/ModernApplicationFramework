using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class WorkaroundMargin : ContentControl,ITextViewMargin
    {
        public WorkaroundMargin()
        {
            IsTabStop = false;
        }

        public FrameworkElement VisualElement => this;

        public bool Enabled => true;

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (marginName != "Workaround")
                return null;
            return this;
        }

        public double MarginSize => 0.0;

        public void Dispose()
        {
        }
    }
}