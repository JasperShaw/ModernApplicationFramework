using System;
using System.Text;
using System.Windows.Input;
using ModernApplicationFramework.Core.Localization;

namespace ModernApplicationFramework.CommandBase.Input
{
    public class KeySequence
    {
        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public KeySequence(ModifierKeys modifiers, Key key)
        {
            if (key == Key.None)
                throw new ArgumentNullException(nameof(key));
            Key = key;
            Modifiers = modifiers;
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
    }
}