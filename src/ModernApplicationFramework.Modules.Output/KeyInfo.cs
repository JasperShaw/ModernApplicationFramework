using System;
using System.Windows.Input;

namespace ModernApplicationFramework.Modules.Output
{
    [Serializable]
    public class KeyInfo
    {
        private static readonly Lazy<KeyInfo> VsKeyInfoReturn = new Lazy<KeyInfo>(() => Create(Key.Return, '\r', 13));

        private KeyInfo()
        {
        }

        public static KeyInfo Create(Key key, char keyChar, byte virtualKey, KeyStates keyStates = KeyStates.None, bool shiftPressed = false, bool controlPressed = false, bool altPressed = false, bool capsLockToggled = false, bool numLockToggled = false)
        {
            return new KeyInfo
            {
                Key = key,
                KeyChar = keyChar,
                VirtualKey = virtualKey,
                KeyStates = keyStates,
                ShiftPressed = shiftPressed,
                ControlPressed = controlPressed,
                AltPressed = altPressed,
                CapsLockToggled = capsLockToggled,
                NumLockToggled = numLockToggled
            };
        }

        public static KeyInfo Enter => VsKeyInfoReturn.Value;

        public Key Key { get; private set; }

        public char KeyChar { get; private set; }

        public byte VirtualKey { get; private set; }

        public KeyStates KeyStates { get; private set; }

        public bool ShiftPressed { get; private set; }

        public bool ControlPressed { get; private set; }

        public bool AltPressed { get; private set; }

        public bool CapsLockToggled { get; private set; }

        public bool NumLockToggled { get; private set; }
    }
}