using System;
using System.Windows.Media;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class BackgroundBrushChangedEventArgs : EventArgs
    {
        public Brush NewBackgroundBrush { get; }

        public BackgroundBrushChangedEventArgs(Brush newBackgroundBrush)
        {
            NewBackgroundBrush = newBackgroundBrush ?? throw new ArgumentNullException(nameof(newBackgroundBrush));
        }     
    }
}