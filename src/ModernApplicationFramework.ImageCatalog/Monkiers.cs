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

        public static Imaging.Interop.ImageMoniker StatusError =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.StatusError
            };

        public static Imaging.Interop.ImageMoniker StatusWarning =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.StatusWarning
            };

        public static Imaging.Interop.ImageMoniker StatusInfo =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.StatusInfo
            };

        public static Imaging.Interop.ImageMoniker CloseDocumentGroup =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.CloseDocumentGroup
            };

        public static Imaging.Interop.ImageMoniker HideToolWindow =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.HideToolWindow
            };

        public static Imaging.Interop.ImageMoniker SplitScreenHorizontal =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.SplitScreenHorizontal
            };

        public static Imaging.Interop.ImageMoniker SplitScreenVertical =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.SplitScreenVertical
            };
    }
}
