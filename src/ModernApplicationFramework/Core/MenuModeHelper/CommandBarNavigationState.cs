using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Core.MenuModeHelper
{
    internal class CommandBarNavigationState
    {
        private CommandBarNavigationDirection _cycleDirection;
        private Key _encounteredTabKey;
        private bool _isInTabCycle;

        private Func<CommandBarNavigationDirection, bool> NavigateCommandFocus { get; }

        public CommandBarNavigationState(Func<CommandBarNavigationDirection, bool> navigateCommandFocus)
        {
            NavigateCommandFocus = navigateCommandFocus;
        }

        public bool FilterKeyDownMessage(Key realKey, ModifierKeys modifierKeys, bool isRepeat)
        {
            var flag1 = false;
            if (MenuModeHelper.IsInCommandMode)
            {
                var flag2 = false;
                if (IsTabKey(realKey) && TestModifierKeys(modifierKeys, ModifierKeys.Control,
                        ModifierKeys.Alt | ModifierKeys.Windows))
                {
                    flag2 = true;
                    _cycleDirection = TestModifierKeys(modifierKeys, ModifierKeys.Shift)
                        ? CommandBarNavigationDirection.Previous
                        : CommandBarNavigationDirection.Next;
                }
                else if (MenuModeHelper.CommandModeKind == CommandBarModeKind.NavigableControl &&
                         TestModifierKeys(modifierKeys, ModifierKeys.None,
                             ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Windows))
                {
                    if (IsTabKey(realKey))
                    {
                        flag2 = true;
                        _cycleDirection = TestModifierKeys(modifierKeys, ModifierKeys.Shift)
                            ? CommandBarNavigationDirection.Previous
                            : CommandBarNavigationDirection.Next;
                    }
                    else if (realKey == Key.Left)
                    {
                        flag2 = true;
                        _cycleDirection = CommandBarNavigationDirection.Previous;
                    }
                    else if (realKey == Key.Right)
                    {
                        flag2 = true;
                        _cycleDirection = CommandBarNavigationDirection.Next;
                    }
                    else if (realKey == Key.Escape)
                    {
                        Keyboard.Focus(null);
                        flag1 = true;
                    }
                }
                if (flag2)
                {
                    if (_encounteredTabKey == Key.None)
                    {
                        _encounteredTabKey = realKey;
                        _isInTabCycle = true;
                    }
                    if (_isInTabCycle)
                        flag1 = NavigateCommandFocus(_cycleDirection);
                }
            }
            else
            {
                _isInTabCycle = false;
                _encounteredTabKey = Key.None;
            }
            return flag1;
        }

        public bool FilterKeyUpMessage(Key realKey)
        {
            if (realKey == _encounteredTabKey)
                _encounteredTabKey = Key.None;
            _isInTabCycle = false;
            return false;
        }

        public void ResetState()
        {
            _encounteredTabKey = Key.None;
            _isInTabCycle = false;
        }

        private static bool IsTabKey(Key key)
        {
            return key == Key.Tab;
        }

        private static bool TestModifierKeys(ModifierKeys modifierKeys, ModifierKeys requiredKeys,
            ModifierKeys disallowedKeys = ModifierKeys.None)
        {
            return (modifierKeys & (requiredKeys | disallowedKeys)) == requiredKeys;
        }
    }
}