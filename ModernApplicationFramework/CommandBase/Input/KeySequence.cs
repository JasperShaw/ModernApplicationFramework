using System;
using System.Globalization;
using System.Text;
using System.Windows.Input;

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
            var modifierName =
                (string)
                new ModifierKeysConverter().ConvertTo(null, CultureInfo.CurrentUICulture, Modifiers,
                    typeof(string));


            if (Modifiers != ModifierKeys.None)
                builder.Append($"{modifierName}+");
            builder.Append(Key);
            return builder.ToString();
        }
    }
}