using System.Resources;
using ModernApplicationFramework.Basics.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="FormattedLocalizableStringConverter"/> for the 'CommandBarResources'
    /// </summary>
    /// <seealso cref="FormattedLocalizableStringConverter" />
    public class CommandBarFormattedStringConverter : FormattedLocalizableStringConverter
	{
	    public override ResourceManager ResourceManager => CommandBarResources.ResourceManager;
    }
}
