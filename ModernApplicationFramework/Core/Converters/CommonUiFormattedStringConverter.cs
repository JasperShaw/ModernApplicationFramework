using System.Resources;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Converters
{
    public class CommonUiFormattedStringConverter : FormattedLocalizableStringConverter
	{
	    public override ResourceManager ResourceManager => CommonUI_Resources.ResourceManager;
    }
}
