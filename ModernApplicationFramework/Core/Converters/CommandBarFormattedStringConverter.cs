using System.Resources;
using ModernApplicationFramework.Basics.CommandBar;

namespace ModernApplicationFramework.Core.Converters
{
    public class CommandBarFormattedStringConverter : FormattedLocalizableStringConverter
	{
	    public override ResourceManager ResourceManager => CommandBarResources.ResourceManager;
    }
}
