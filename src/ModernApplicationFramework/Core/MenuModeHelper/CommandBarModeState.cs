using System;
using System.Windows.Input;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.MenuModeHelper
{
    internal class CommandBarModeState
    {
        public Key LastKeyPressed;
        private ModifierKeys _lastModifierKeys;
        private bool _shouldEnterToolBarMode;
        private bool _win32MenuModeWorkAround;

        private Func<bool> TryEnterMenuMode { get; }

        private Func<bool> TryEnterToolBarMode { get; }

        public CommandBarModeState(Func<bool> enterMenuMode, Func<bool> enterToolBarMode)
        {
            TryEnterMenuMode = enterMenuMode;
            TryEnterToolBarMode = enterToolBarMode;
        }

        public bool FilterKeyDownMessage(Key realKey, ModifierKeys modifierKeys, bool isRepeat)
        {
            var commandModeKind = MenuModeHelper.CommandModeKind;
            var flag = commandModeKind != CommandBarModeKind.None && commandModeKind != CommandBarModeKind.Menu;
            if (commandModeKind == CommandBarModeKind.Menu ||
                flag && !ShouldReenterMenuCommandMode(realKey, modifierKeys))
            {
                ClearLastKey();
            }
            else if (!isRepeat)
            {
                if (LastKeyPressed == Key.None)
                {
                    if (IsMenuKey(realKey) && !NativeMethods.IsLeftButtonPressed() &&
                        !NativeMethods.IsRightButtonPressed())
                        if ((modifierKeys & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows)) ==
                            ModifierKeys.Shift)
                        {
                            if (realKey != Key.F10)
                            {
                                LastKeyPressed = realKey;
                                _lastModifierKeys = modifierKeys;
                                _shouldEnterToolBarMode = true;
                            }
                        }
                        else if ((modifierKeys & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Windows)) ==
                                 ModifierKeys.None)
                        {
                            LastKeyPressed = realKey;
                            _lastModifierKeys = modifierKeys;
                        }
                }
                else if (LastKeyPressed != realKey || _lastModifierKeys != modifierKeys)
                {
                    ClearLastKey();
                }
                _win32MenuModeWorkAround = false;
            }
            return false;
        }

        public bool FilterKeyUpMessage(Key realKey)
        {
            var flag = false;
            if (realKey == LastKeyPressed && IsMenuKey(realKey))
            {
                if (_shouldEnterToolBarMode)
                    flag = TryEnterToolBarMode();
                if (!flag)
                    flag = TryEnterMenuMode();
            }
            if (_win32MenuModeWorkAround)
            {
                if (IsMenuKey(realKey))
                {
                    _win32MenuModeWorkAround = false;
                    flag = true;
                }
            }
            else if (flag)
            {
                _win32MenuModeWorkAround = true;
            }
            ClearLastKey();
            return flag;
        }

        public bool FilterMouseMessage()
        {
            ClearLastKey();
            _win32MenuModeWorkAround = false;
            return false;
        }

        public void ResetState()
        {
            ClearLastKey();
            _win32MenuModeWorkAround = false;
        }

        private static bool IsMenuKey(Key key)
        {
            if (key != Key.LeftAlt && key != Key.RightAlt)
                return key == Key.F10;
            return true;
        }

        private void ClearLastKey()
        {
            LastKeyPressed = Key.None;
            _shouldEnterToolBarMode = false;
        }

        private bool ShouldReenterMenuCommandMode(Key realKey, ModifierKeys modifierKeys)
        {
            return (realKey == Key.LeftAlt || realKey == Key.RightAlt) && modifierKeys == ModifierKeys.Alt ||
                   realKey == Key.F10 && (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
        }
    }
}