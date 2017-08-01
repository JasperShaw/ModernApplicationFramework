using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Localization
{
    public static class KeyboardLocalizationUtilities
    {
        private const char ModifierDelimiter = '+';

        private static readonly ResourceManager ResourceManager = KeyboardLocalizationResources.ResourceManager;

        public static string GetCultureModifiersName(ModifierKeys modifiers, CultureInfo culture = null)
        {
            if (!ModifierKeysConverter.IsDefinedModifierKeys(modifiers))
                throw new InvalidEnumArgumentException(nameof(modifiers), (int) modifiers, typeof(ModifierKeys));
            var str = string.Empty;

            if ((modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                str += GetSingleCultureModifierName(ModifierKeys.Control);
            if ((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                if (str.Length > 0)
                    str += ModifierDelimiter;
                str += GetSingleCultureModifierName(ModifierKeys.Shift);
            }
            if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                if (str.Length > 0)
                    str += ModifierDelimiter;
                str += GetSingleCultureModifierName(ModifierKeys.Alt);
            }
            if ((modifiers & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                if (str.Length > 0)
                    str += ModifierDelimiter;
                str += GetSingleCultureModifierName(ModifierKeys.Windows);
            }
            return str;
        }

        public static string GetSingleCultureModifierName(ModifierKeys modifier, CultureInfo culture = null)
        {
            switch (modifier)
            {
                case ModifierKeys.None:
                    return string.Empty;
                case ModifierKeys.Alt:
                    return GetResource("Alt", culture);
                case ModifierKeys.Control:
                    return GetResource("Control", culture);
                case ModifierKeys.Shift:
                    return GetResource("Shift", culture);
                case ModifierKeys.Windows:
                    return GetResource("Windows", culture);
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }

        public static string GetKeyCultureName(Key key, CultureInfo culture = null)
        {
            var ret = GetLocalizedKeyName(key, culture);
            if (!string.IsNullOrEmpty(ret))
                return ret;     
            //Translate OEM keys
            ret = KeyCodeToUnicode(key);
            return ret;
        }

        public static ModifierKeys SingleStringToModifierKey(string name, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(name))
                return ModifierKeys.None;
            if (name == GetResource("Alt", culture))
                return ModifierKeys.Alt;
            if (name == GetResource("Control", culture))
                return ModifierKeys.Control;
            if (name == GetResource("Shift", culture))
                return ModifierKeys.Shift;
            if (name == GetResource("Windows", culture))
                return ModifierKeys.Windows;          
            return ModifierKeys.None;
        }

        public static Key CultureStringToKey(string keyString, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(keyString))
                return Key.None;
            var ret = GetKeyByLocalizedName(keyString, culture);
            if (ret != Key.None)
                return ret;
            var tmp = keyString.ToCharArray();         
            if (tmp.Length > 1)
                throw new ArgumentException($"Invalid key string: {keyString}");       
            return KeyInterop.KeyFromVirtualKey(VkKeyScan(tmp[0]));
        }

        private static string GetLocalizedKeyName(Key key, CultureInfo culture)
        {
            //Keys from A to Z
            if ((int)key >= 44 && (int)key <= 69)
                return key.ToString();

            //Other Special Keys
            if (key == Key.Cancel)
                return GetResource("Cancel", culture);
            if (key == Key.Back)
                return GetResource("Backspace", culture);
            if (key == Key.Tab)
                return GetResource("Tab", culture);
            if (key == Key.Clear)
                return GetResource("Clear", culture);
            if (key == Key.Enter || key == Key.Return)
                return GetResource("Enter", culture);
            if (key == Key.Escape)
                return GetResource("Escape", culture);
            if (key == Key.Space)
                return GetResource("Space", culture);
            if (key == Key.PageUp || key == Key.Prior)
                return GetResource("PageUp", culture);
            if (key == Key.PageDown || key == Key.Next)
                return GetResource("PageDown", culture);
            if (key == Key.End)
                return GetResource("End", culture);
            if (key == Key.Left)
                return GetResource("Left", culture);
            if (key == Key.Up)
                return GetResource("Up", culture);
            if (key == Key.Right)
                return GetResource("Right", culture);
            if (key == Key.Down)
                return GetResource("Down", culture);
            if (key == Key.Insert)
                return GetResource("Insert", culture);
            if (key == Key.Delete)
                return GetResource("Delete", culture);
            if (key == Key.Pause)
                return GetResource("Pause", culture);
            if (key == Key.LWin)
                return GetResource("LeftWindows", culture);
            if (key == Key.RWin)
                return GetResource("RightWindows", culture);
            if (key == Key.NumPad0)
                return GetResource("NumPad0", culture);
            if (key == Key.NumPad1)
                return GetResource("NumPad1", culture);
            if (key == Key.NumPad2)
                return GetResource("NumPad2", culture);
            if (key == Key.NumPad3)
                return GetResource("NumPad3", culture);
            if (key == Key.NumPad4)
                return GetResource("NumPad4", culture);
            if (key == Key.NumPad5)
                return GetResource("NumPad5", culture);
            if (key == Key.NumPad6)
                return GetResource("NumPad6", culture);
            if (key == Key.NumPad7)
                return GetResource("NumPad7", culture);
            if (key == Key.NumPad8)
                return GetResource("NumPad8", culture);
            if (key == Key.NumPad9)
                return GetResource("NumPad9", culture);
            if (key == Key.Multiply)
                return GetResource("Multiply", culture);
            if (key == Key.Add)
                return GetResource("Add", culture);
            if (key == Key.Subtract)
                return GetResource("Subtract", culture);
            if (key == Key.Decimal)
                return GetResource("Decimal", culture);
            if (key == Key.Multiply)
                return GetResource("Divide", culture);
            if (key == Key.F1)
                return GetResource("F1", culture);
            if (key == Key.F2)
                return GetResource("F2", culture);
            if (key == Key.F3)
                return GetResource("F3", culture);
            if (key == Key.F4)
                return GetResource("F4", culture);
            if (key == Key.F5)
                return GetResource("F5", culture);
            if (key == Key.F6)
                return GetResource("F6", culture);
            if (key == Key.F7)
                return GetResource("F7", culture);
            if (key == Key.F8)
                return GetResource("F8", culture);
            if (key == Key.F9)
                return GetResource("F9", culture);
            if (key == Key.F10)
                return GetResource("F10", culture);
            if (key == Key.F11)
                return GetResource("F11", culture);
            if (key == Key.F12)
                return GetResource("F12", culture);
            if (key == Key.F13)
                return GetResource("F13", culture);
            if (key == Key.F14)
                return GetResource("F14", culture);
            if (key == Key.F15)
                return GetResource("F15", culture);
            if (key == Key.F16)
                return GetResource("F16", culture);
            if (key == Key.F17)
                return GetResource("F17", culture);
            if (key == Key.F18)
                return GetResource("F18", culture);
            if (key == Key.F19)
                return GetResource("F19", culture);
            if (key == Key.F20)
                return GetResource("F20", culture);
            if (key == Key.F21)
                return GetResource("F21", culture);
            if (key == Key.F22)
                return GetResource("F22", culture);
            if (key == Key.F23)
                return GetResource("F23", culture);
            if (key == Key.F24)
                return GetResource("F24", culture);
            return string.Empty;
        }

        private static Key GetKeyByLocalizedName(string name, CultureInfo culture)
        {
            if (string.Equals(name, GetResource("Cancel", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Cancel;
            if (string.Equals(name, GetResource("Backspace", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Back;
            if (string.Equals(name, GetResource("Tab", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Tab;
            if (string.Equals(name, GetResource("Clear", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Clear;
            if (string.Equals(name, GetResource("Enter", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Enter;
            if (string.Equals(name, GetResource("Escape", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Escape;
            if (string.Equals(name, GetResource("Space", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Space;
            if (string.Equals(name, GetResource("PageUp", culture), StringComparison.OrdinalIgnoreCase))
                return Key.PageUp;
            if (string.Equals(name, GetResource("PageDown", culture), StringComparison.OrdinalIgnoreCase))
                return Key.PageDown;
            if (string.Equals(name, GetResource("End", culture), StringComparison.OrdinalIgnoreCase))
                return Key.End;
            if (string.Equals(name, GetResource("Left", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Left;
            if (string.Equals(name, GetResource("Up", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Up;
            if (string.Equals(name, GetResource("Right", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Right;
            if (string.Equals(name, GetResource("Down", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Down;
            if (string.Equals(name, GetResource("Insert", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Insert;
            if (string.Equals(name, GetResource("Delete", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Delete;
            if (string.Equals(name, GetResource("Pause", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Pause;
            if (string.Equals(name, GetResource("LeftWindows", culture), StringComparison.OrdinalIgnoreCase))
                return Key.LWin;
            if (string.Equals(name, GetResource("RightWindows", culture), StringComparison.OrdinalIgnoreCase))
                return Key.RWin;
            if (string.Equals(name, GetResource("NumPad0", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad0;
            if (string.Equals(name, GetResource("NumPad1", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad1;
            if (string.Equals(name, GetResource("NumPad2", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad2;
            if (string.Equals(name, GetResource("NumPad3", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad3;
            if (string.Equals(name, GetResource("NumPad4", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad4;
            if (string.Equals(name, GetResource("NumPad5", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad5;
            if (string.Equals(name, GetResource("NumPad6", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad6;
            if (string.Equals(name, GetResource("NumPad7", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad7;
            if (string.Equals(name, GetResource("NumPad8", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad8;
            if (string.Equals(name, GetResource("NumPad9", culture), StringComparison.OrdinalIgnoreCase))
                return Key.NumPad9;
            if (string.Equals(name, GetResource("Multiply", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Multiply;
            if (string.Equals(name, GetResource("Add", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Add;
            if (string.Equals(name, GetResource("Subtract", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Subtract;
            if (string.Equals(name, GetResource("Decimal", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Decimal;
            if (string.Equals(name, GetResource("Divide", culture), StringComparison.OrdinalIgnoreCase))
                return Key.Divide;
            if (string.Equals(name, GetResource("F1", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F1;
            if (string.Equals(name, GetResource("F2", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F2;
            if (string.Equals(name, GetResource("F3", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F3;
            if (string.Equals(name, GetResource("F4", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F4;
            if (string.Equals(name, GetResource("F5", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F5;
            if (string.Equals(name, GetResource("F6", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F6;
            if (string.Equals(name, GetResource("F7", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F7;
            if (string.Equals(name, GetResource("F8", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F8;
            if (string.Equals(name, GetResource("F9", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F9;
            if (string.Equals(name, GetResource("F10", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F10;
            if (string.Equals(name, GetResource("F11", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F11;
            if (string.Equals(name, GetResource("F12", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F12;
            if (string.Equals(name, GetResource("F13", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F13;
            if (string.Equals(name, GetResource("F14", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F14;
            if (string.Equals(name, GetResource("F15", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F15;
            if (string.Equals(name, GetResource("F16", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F16;
            if (string.Equals(name, GetResource("F17", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F17;
            if (string.Equals(name, GetResource("F18", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F18;
            if (string.Equals(name, GetResource("F19", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F19;
            if (string.Equals(name, GetResource("F20", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F20;
            if (string.Equals(name, GetResource("F21", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F21;
            if (string.Equals(name, GetResource("F22", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F22;
            if (string.Equals(name, GetResource("F23", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F23;
            if (string.Equals(name, GetResource("F24", culture), StringComparison.OrdinalIgnoreCase))
                return Key.F24;
            return Key.None;
        }

        private static string KeyCodeToUnicode(Key key)
        {
            // ToUnicodeEx needs StringBuilder, it populates that during execution.
            var sbString = new StringBuilder();

            var bKeyState = new byte[255];
            var bKeyStateStatus = User32.GetKeyboardState(bKeyState);

            // On failure we return empty string.
            if (!bKeyStateStatus)
                return "";

            // Gets the layout of keyboard
            var hkl = User32.GetKeyboardLayout(0);

            // Maps the virtual keycode
            var vkCode = (uint)KeyInterop.VirtualKeyFromKey(key);
            uint lScanCode = User32.MapVirtualKeyEx(vkCode, 0, hkl);

            // Converts the VKCode to unicode
            var relevantKeyCountInBuffer =
                User32.ToUnicodeEx(vkCode, lScanCode, new byte[255], sbString, sbString.Capacity, 0, hkl);

            var ret = string.Empty;

            switch (relevantKeyCountInBuffer)
            {
                // Dead keys (^,`...)
                case -1:
                    // We must clear the buffer because ToUnicodeEx messed it up, see below.
                    ClearKeyboardBuffer(vkCode, lScanCode, hkl);
                    ret = sbString[0].ToString();
                    break;
                case 0:
                    break;

                // Single character in buffer
                case 1:
                    ret = sbString[0].ToString();
                    break;
            }
            return ret;
        }

        private static void ClearKeyboardBuffer(uint vk, uint sc, IntPtr hkl)
        {
            var sb = new StringBuilder();
            int rc;
            do
                rc = User32.ToUnicodeEx(vk, sc, new byte[255], sb, 10, 0, hkl);
            while (rc < 0);
        }

        private static string GetResource(string key, CultureInfo culture = null)
        {
            return ResourceManager.GetString($"KeyboardKey_{key}", culture);
        }

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);
    }
}
