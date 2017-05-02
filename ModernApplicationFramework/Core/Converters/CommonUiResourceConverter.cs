using System.Resources;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Converters
{
	public class CommonUiResourceConverter : LocalizableResourceConverter
	{
		public override ResourceManager ResourceManager => CommonUI_Resources.ResourceManager;
	}
}