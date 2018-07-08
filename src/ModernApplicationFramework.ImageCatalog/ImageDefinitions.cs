using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    internal static class ImageDefinitions
    {
        public static ImageDefinition UndoDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Undo,
            Source = UriCreator.Create("Undo_16x", ImageType.Xaml)
        };

        public static ImageDefinition StatusInfoDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.StatusInfo,
            Source = UriCreator.Create("StatusInformation_16x", ImageType.Xaml)
        };

        public static ImageDefinition StatusWarningDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.StatusWarning,
            Source = UriCreator.Create("StatusWarning_16x", ImageType.Xaml)
        };

        public static ImageDefinition StatusErrorDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.StatusError,
            Source = UriCreator.Create("StatusCriticalError_16x", ImageType.Xaml)
        };

        public static ImageDefinition CloseDocumentGroupDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.CloseDocumentGroup,
            Source = UriCreator.Create("CloseDocumentGroup", ImageType.Xaml)
        };

        public static ImageDefinition HideToolWindowDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.HideToolWindow,
            Source = UriCreator.Create("HideToolWindow", ImageType.Xaml)
        };

        public static ImageDefinition SplitScreenHorizontalDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.SplitScreenHorizontal,
            Source = UriCreator.Create("SplitScreenHorizontal_16x", ImageType.Xaml)
        };

        public static ImageDefinition SplitScreenVerticalDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.SplitScreenVertical,
            Source = UriCreator.Create("SplitScreenVertical_16x", ImageType.Xaml)
        };
    }
}
