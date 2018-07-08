using ModernApplicationFramework.Basics.Imaging;

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
    }
}
