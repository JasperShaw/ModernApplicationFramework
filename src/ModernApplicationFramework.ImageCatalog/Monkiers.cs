using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    public static class Monikers
    {
        public static Imaging.Interop.ImageMoniker Undo =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Undo
            };
    }
}
