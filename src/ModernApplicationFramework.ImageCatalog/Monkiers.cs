using ModernApplicationFramework.Basics.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    public static class Monkiers
    {
        public static ImageMonkier Undo =>
            new ImageMonkier(FrameworkImageCatalog.FrameworkImageCatalogGuid, ImageIds.Undo);
    }
}
