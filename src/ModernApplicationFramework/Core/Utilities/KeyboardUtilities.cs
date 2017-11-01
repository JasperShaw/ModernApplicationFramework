using System.Windows.Input;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class KeyboardUtilities
    {
        internal static bool IsKeyModifyingPopupState(KeyEventArgs e)
        {
            return (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt &&
                   (e.SystemKey == Key.Down || e.SystemKey == Key.Up) || e.Key == Key.F4;
        }

        internal static Key GetRealPressedKey(KeyEventArgs e)
        {
            if (e == null)
                return Key.None;
            return e.Key == Key.System ? e.SystemKey : e.Key;
        }
    }
}