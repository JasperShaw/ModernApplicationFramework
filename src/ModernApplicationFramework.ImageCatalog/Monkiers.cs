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

        public static Imaging.Interop.ImageMoniker CloseProgram =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.CloseProgram
            };

        public static Imaging.Interop.ImageMoniker Copy =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Copy
            };

        public static Imaging.Interop.ImageMoniker Cut =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Cut
            };

        public static Imaging.Interop.ImageMoniker Delete =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Delete
            };

        public static Imaging.Interop.ImageMoniker FitToScreen =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.FitToScreen
            };

        public static Imaging.Interop.ImageMoniker MoveDown =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.MoveDown
            };

        public static Imaging.Interop.ImageMoniker MoveUp =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.MoveUp
            };

        public static Imaging.Interop.ImageMoniker Paste =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Paste
            };

        public static Imaging.Interop.ImageMoniker Redo =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Redo
            };

        public static Imaging.Interop.ImageMoniker Settings =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Settings
            };

        public static Imaging.Interop.ImageMoniker OpenFolder =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.OpenFolder
            };

        public static Imaging.Interop.ImageMoniker Save =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Save
            };

        public static Imaging.Interop.ImageMoniker SaveAll =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.SaveAll
            };

        public static Imaging.Interop.ImageMoniker NewFile =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.NewFile
            };

        public static Imaging.Interop.ImageMoniker Property =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Property
            };

        public static Imaging.Interop.ImageMoniker Output =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Output
            };

        public static Imaging.Interop.ImageMoniker Toolbox =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Toolbox
            };

        public static Imaging.Interop.ImageMoniker Win32Text =>
            new Imaging.Interop.ImageMoniker
            {
                CatalogGuid = FrameworkImageCatalog.FrameworkImageCatalogGuid,
                Id = ImageIds.Win32Text
            };
    }
}
