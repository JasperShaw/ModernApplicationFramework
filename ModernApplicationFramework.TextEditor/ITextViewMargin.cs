using System;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewMargin : IDisposable
    {
        FrameworkElement VisualElement { get; }

        double MarginSize { get; }

        bool Enabled { get; }

        ITextViewMargin GetTextViewMargin(string marginName);
    }
}