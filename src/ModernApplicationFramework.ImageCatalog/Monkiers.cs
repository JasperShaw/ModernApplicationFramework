using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    public static class Monikers
    {
        public static ImageMoniker Undo =>
            new ImageMoniker(FrameworkImageCatalog.FrameworkImageCatalogGuid, ImageIds.Undo);
    }
}
