using System.Windows.Media;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class ViewWrapProperties
    {
        public Brush ForegroundBrush { get; }

        public double Width { get; }

        public ViewWrapProperties(double width, Brush foregroundBrush)
        {
            Width = width;
            ForegroundBrush = foregroundBrush;
            if (!ForegroundBrush.CanFreeze)
                return;
            ForegroundBrush.Freeze();
        }
    }
}