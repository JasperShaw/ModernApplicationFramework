﻿using Microsoft.Win32;
using ModernApplicationFramework.Modules.Editor.NativeMethods;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal static class CaretBlinkTimeManager
    {
        private static int _blinkTime = User32.GetCaretBlinkTime();

        static CaretBlinkTimeManager()
        {
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        internal static int GetCaretBlinkTime()
        {
            return _blinkTime;
        }

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category != UserPreferenceCategory.Keyboard)
                return;
            _blinkTime = User32.GetCaretBlinkTime();
        }
    }
}