using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal class ViewWrapProperties
    {
        public ViewWrapProperties(double width, Brush foregroundBrush)
        {
            Width = width;
            ForegroundBrush = foregroundBrush;
            if (!ForegroundBrush.CanFreeze)
                return;
            ForegroundBrush.Freeze();
        }

        public double Width { get; }

        public Brush ForegroundBrush { get; }
    }
}