using System;
using System.Text;
using System.Windows.Input;

namespace ModernApplicationFramework.Input
{
    public class KeySequence
    {
        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public KeySequence(Key key) : this(ModifierKeys.None, key) { }

        public KeySequence(ModifierKeys modifiers, Key key)
        {
            if (key == Key.None)
                throw new ArgumentException(nameof(key));
            Key = key;
            Modifiers = modifiers;
        }
        
        public bool IsValid(ValidCheckMode mode = ValidCheckMode.FirstSequence)
        {
            if ((Key < Key.F1 || Key > Key.F24) && (Key < Key.NumPad0 || Key > Key.Divide))
            {
                if ((Modifiers & (ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Windows)) != ModifierKeys.None)
                {
                    switch (Key)
                    {
                        case Key.LWin:
                        case Key.RWin:
                        case Key.LeftCtrl:
                        case Key.RightCtrl:
                        case Key.LeftAlt:
                        case Key.RightAlt:
                            return false;
                        default:
                            return true;
                    }
                }
                if (Key >= Key.D0 && Key <= Key.D9 || (Key >= Key.A && Key <= Key.Z && mode == ValidCheckMode.FirstSequence))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var modifierName = KeyboardLocalizationUtilities.GetCultureModifiersName(Modifiers);


            if (Modifiers != ModifierKeys.None)
                builder.Append($"{modifierName}+");

            builder.Append(KeyboardLocalizationUtilities.GetKeyCultureName(Key));
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is KeySequence sequence)
                return Equals(sequence);
            return false;
        }

        protected bool Equals(KeySequence sequence)
        {
            if (Key == sequence.Key && Modifiers == sequence.Modifiers)
                return true;
            return false;
        }
    }
}