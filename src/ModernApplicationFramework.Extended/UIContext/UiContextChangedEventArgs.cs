using System;

namespace ModernApplicationFramework.Extended.UIContext
{
    public sealed class UiContextChangedEventArgs : EventArgs
    {
        private static readonly UiContextChangedEventArgs UiContextActivated = new UiContextChangedEventArgs(true);
        private static readonly UiContextChangedEventArgs UiContextDeactivated = new UiContextChangedEventArgs(false);

        public bool Activated { get; }

        public static UiContextChangedEventArgs From(bool activated)
        {
            if (!activated)
                return UiContextDeactivated;
            return UiContextActivated;
        }

        public UiContextChangedEventArgs(bool activated)
        {
            Activated = activated;
        }
    }
}