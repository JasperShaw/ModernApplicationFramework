using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextView
    {
        event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        event EventHandler Closed;

        FrameworkElement VisualElement { get; }

        Brush Background { get; set; }

        bool IsClosed { get; }

        void Close();
    }
}