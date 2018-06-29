using System;
using ModernApplicationFramework.Basics.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    internal static class UriCreator
    {
        public static Uri Create(string fileName, ImageType type)
        {
            return new Uri($"/ModernApplicationFramework.ImageCatalog;component/Resources/{type.ToString()}/{fileName}.xaml");
        }
    }
}
