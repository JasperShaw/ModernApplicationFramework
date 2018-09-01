using System;
using System.Windows;

namespace ModernApplicationFramework.Docking
{
    internal static class UtilityMethods
    {
        public static Rect Resize(this Rect rect, Vector positionChangeDelta, Vector sizeChangeDelta, Size minSize, Size maxSize)
        {
            var width = Math.Min(Math.Max(minSize.Width, rect.Width + sizeChangeDelta.X), maxSize.Width);
            var height = Math.Min(Math.Max(minSize.Height, rect.Height + sizeChangeDelta.Y), maxSize.Height);
            var right = rect.Right;
            var bottom = rect.Bottom;
            return new Rect(Math.Min(right - minSize.Width, Math.Max(right - maxSize.Width, rect.Left + positionChangeDelta.X)), Math.Min(bottom - minSize.Height, Math.Max(bottom - maxSize.Height, rect.Top + positionChangeDelta.Y)), width, height);
        }

        internal static void AddPresentationSourceCleanupAction(UIElement element, Action handler)
        {
            void RelayHandler(object sender, SourceChangedEventArgs args)
            {
                if (args.NewSource != null) return;
                if (!element.Dispatcher.HasShutdownStarted) handler();
                PresentationSource.RemoveSourceChangedHandler(element, RelayHandler);
            }

            PresentationSource.AddSourceChangedHandler(element, RelayHandler);
        }
    }
}
