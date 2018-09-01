using System;
using System.Windows;

namespace ModernApplicationFramework.Docking
{
    internal static class UtilityMethods
    {
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
