using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.Extended.Demo.ImageCatalog
{
    [Export(typeof(IImageCatalog))]
    public class ImageCatalog : IImageCatalog
    {
        public static Guid ImageCatalogGuid = new Guid("{E86E9E26-A500-498B-AED3-416817AF3735}");
        public Guid ImageCataloGuid => ImageCatalogGuid;
        public bool GetDefinition(int id, out ImageDefinition imageDefinition)
        {
            imageDefinition = default;
            if (id == 0)
            {
                imageDefinition = new ImageDefinition
                {
                    Monkier = Monikers.Add,
                    Type = ImageType.Png,
                    Source = UriCreator.Create("action_add_16xLG", ImageType.Png)
                };
                return true;
            }

            return false;
        }
    }
}
