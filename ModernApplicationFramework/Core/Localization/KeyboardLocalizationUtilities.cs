using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
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

        public static string GetCultureModifierName(ModifierKeys modifiers, CultureInfo culture = null)
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

        public static string GetKeyCultureName(Key key)
        {
            var ret = PreFilterKeys(key);
            if (!string.IsNullOrEmpty(ret))
                return ret;     
            //Translate OEM keys
            ret = KeyCodeToUnicode(key);
            return ret;
        }

        private static string PreFilterKeys(Key key)
        {
            //Keys from A to Z
            if ((int)key >= 44 && (int)key <= 96)
                return key.ToString();
            
            //Other Special Keys
            if (key == Key.Cancel)
                return GetResource("Cancel");
            if (key == Key.Back)
                return GetResource("Back");
            if (key == Key.Tab)
                return GetResource("Tab");
            if (key == Key.Clear)
                return GetResource("Clear");
            if (key == Key.Enter || key == Key.Return)
                return GetResource("Enter");
            if (key == Key.Escape)
                return GetResource("Escape");
            if (key == Key.Space)
                return GetResource("Space");
            if (key == Key.PageUp || key == Key.Prior)
                return GetResource("PageUp");
            if (key == Key.PageDown || key == Key.Next)
                return GetResource("PageDown");
            if (key == Key.End)
                return GetResource("End");
            if (key == Key.Left)
                return GetResource("Left");
            if (key == Key.Up)
                return GetResource("Up");
            if (key == Key.Right)
                return GetResource("Right");
            if (key == Key.Down)
                return GetResource("Down");
            if (key == Key.Insert)
                return GetResource("Insert");
            if (key == Key.Delete)
                return GetResource("Delete");
            if (key == Key.Pause)
                return GetResource("Pause");
            if (key == Key.LWin)
                return GetResource("LeftWindows");
            if (key == Key.RWin)
                return GetResource("RightWindows");
            if (key == Key.NumPad0)
                return GetResource("NumPad0");
            if (key == Key.NumPad1)
                return GetResource("NumPad1");
            if (key == Key.NumPad2)
                return GetResource("NumPad2");
            if (key == Key.NumPad3)
                return GetResource("NumPad3");
            if (key == Key.NumPad4)
                return GetResource("NumPad4");
            if (key == Key.NumPad5)
                return GetResource("NumPad5");
            if (key == Key.NumPad6)
                return GetResource("NumPad6");
            if (key == Key.NumPad7)
                return GetResource("NumPad7");
            if (key == Key.NumPad8)
                return GetResource("NumPad8");
            if (key == Key.NumPad9)
                return GetResource("NumPad9");
            if (key == Key.Multiply)
                return GetResource("Multiply");
            if (key == Key.Add)
                return GetResource("Add");
            if (key == Key.Subtract)
                return GetResource("Subtract");
            if (key == Key.Decimal)
                return GetResource("Decimal");
            if (key == Key.Multiply)
                return GetResource("Divide");
            return string.Empty;
        }

        public static string KeyCodeToUnicode(Key key)
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
    }
}
