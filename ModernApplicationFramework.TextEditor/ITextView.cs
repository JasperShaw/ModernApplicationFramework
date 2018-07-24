using System;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextView
    {
        event EventHandler<BackgroundBrushChangedEventArgs> BackgroundBrushChanged;

        event EventHandler ViewportLeftChanged;

        event EventHandler ViewportHeightChanged;

        event EventHandler ViewportWidthChanged;

        event EventHandler Closed;

        FrameworkElement VisualElement { get; }

        Brush Background { get; set; }

        bool IsClosed { get; }

        double ViewportLeft { get; set; }

        double ViewportTop { get; }

        double ViewportRight { get; }

        double ViewportBottom { get; }

        double ViewportWidth { get; }

        double ViewportHeight { get; }

        void Close();

        ITextSnapshot TextSnapshot { get; }

        ITextViewModel TextViewModel { get; }

        ITextDataModel TextDataModel { get; }
    }
}