using System;
using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    internal static class UriCreator
    {
        public static Uri Create(string fileName, ImageType type)
        {
            var prefix = string.Empty;
            if (type == ImageType.Png)
                prefix = "pack://application:,,,";
            return new Uri(
                $"{prefix}/ModernApplicationFramework.ImageCatalog;component/Resources/{type.ToString()}/{fileName}.{type.ToString().ToLowerInvariant()}",
                UriKind.RelativeOrAbsolute);
        }
    }
}
