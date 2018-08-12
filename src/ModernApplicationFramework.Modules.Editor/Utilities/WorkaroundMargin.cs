using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal sealed class WorkaroundMargin : ContentControl, ITextViewMargin
    {
        public bool Enabled => true;

        public double MarginSize => 0.0;

        public FrameworkElement VisualElement => this;

        public WorkaroundMargin()
        {
            IsTabStop = false;
        }

        public void Dispose()
        {
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (marginName != "Workaround")
                return null;
            return this;
        }
    }
}