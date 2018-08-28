using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal class StylingFlagsConverter : ValueConverter<CommandBarFlags, CommandBarFlags>
    {
        public static CommandBarFlags StylingMask;

        static StylingFlagsConverter()
        {
            StylingMask = CommandBarFlags.CommandFlagPictAndText;
        }

        protected override CommandBarFlags Convert(CommandBarFlags value, object parameter, CultureInfo culture)
        {
            return value & StylingMask;
        }
    }
}
