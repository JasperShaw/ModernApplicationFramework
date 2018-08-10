using System;
using System.Windows;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ITextViewMargin : IDisposable
    {
        bool Enabled { get; }

        double MarginSize { get; }
        FrameworkElement VisualElement { get; }

        ITextViewMargin GetTextViewMargin(string marginName);
    }
}