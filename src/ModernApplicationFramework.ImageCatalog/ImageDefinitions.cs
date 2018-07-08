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
    }
}
