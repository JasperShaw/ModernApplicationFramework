using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public sealed class BottomRightCornerMargin : Border, ITextViewMargin
    {
        public const string MarginName = "BottomRightCornerMargin";

        public FrameworkElement VisualElement => this;

        public bool Enabled => true;

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.IsNullOrEmpty(marginName))
                return null;
            return marginName.Equals("BottomRightCornerMargin", StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public double MarginSize => 0.0;

        public void Dispose()
        {
        }
    }
}