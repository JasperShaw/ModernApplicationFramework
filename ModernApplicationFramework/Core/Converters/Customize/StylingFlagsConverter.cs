using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal class StylingFlagsConverter : ValueConverter<uint, CommandBarFlags>
    {
        public static CommandBarFlags StylingMask;

        static StylingFlagsConverter()
        {
            StylingMask = CommandBarFlags.CommandFlagPictAndText;
        }

        protected override CommandBarFlags Convert(uint value, object parameter, CultureInfo culture)
        {
            return (CommandBarFlags) value & StylingMask;
        }
    }
}
