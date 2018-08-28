using System;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Customize;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class FlagToNameConverter : ValueConverter<CommandBarFlags, string>
    {
        protected override string Convert(CommandBarFlags value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case CommandBarFlags.CommandFlagNone:
                    return Customize_Resources.DefaultStyleCommandFlag;
                case CommandBarFlags.CommandFlagPict:
                    return Customize_Resources.TextOnlyAlwaysCommandFlag;
                case CommandBarFlags.CommandFlagText:
                    return Customize_Resources.TextOnlyAlwaysCommandFlag;
                case CommandBarFlags.CommandFlagPictAndText:
                    return Customize_Resources.ImageAndTextCommandFlag;
            }

            return string.Empty;
        }
    }
}