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

        public static ImageDefinition CloseProgramDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.CloseProgram,
            Source = UriCreator.Create("CloseProgram_16x", ImageType.Xaml)
        };

        public static ImageDefinition CopyDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Copy,
            Source = UriCreator.Create("Copy_16x", ImageType.Xaml)
        };

        public static ImageDefinition CutDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Cut,
            Source = UriCreator.Create("Cut_16x", ImageType.Xaml)
        };

        public static ImageDefinition DeleteDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Delete,
            Source = UriCreator.Create("Delete_16x", ImageType.Xaml)
        };

        public static ImageDefinition FitToScreenDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.FitToScreen,
            Source = UriCreator.Create("AutosizeOptimize_16x", ImageType.Xaml)
        };

        public static ImageDefinition MoveDownDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.MoveDown,
            Source = UriCreator.Create("MoveDown", ImageType.Xaml)
        };

        public static ImageDefinition MoveUpDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.MoveUp,
            Source = UriCreator.Create("MoveUp", ImageType.Xaml)
        };

        public static ImageDefinition PasteDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Paste,
            Source = UriCreator.Create("Paste_16x", ImageType.Xaml)
        };

        public static ImageDefinition RedoDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Redo,
            Source = UriCreator.Create("Redo_16x", ImageType.Xaml)
        };

        public static ImageDefinition SettingsDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Settings,
            Source = UriCreator.Create("Settings_16x", ImageType.Xaml)
        };

        public static ImageDefinition NewFileDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.NewFile,
            Source = UriCreator.Create("VSO_NewFile_16x", ImageType.Xaml)
        };

        public static ImageDefinition OpenFolderDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.OpenFolder,
            Source = UriCreator.Create("OpenFolder_16x", ImageType.Xaml)
        };

        public static ImageDefinition SaveDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Save,
            Source = UriCreator.Create("Save_16x", ImageType.Xaml)
        };

        public static ImageDefinition SaveAllDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.SaveAll,
            Source = UriCreator.Create("SaveAll_16x", ImageType.Xaml)
        };

        public static ImageDefinition PropertyDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Property,
            Source = UriCreator.Create("Property_16x", ImageType.Xaml)
        };

        public static ImageDefinition OutputDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Output,
            Source = UriCreator.Create("Output_16x", ImageType.Xaml)
        };

        public static ImageDefinition ToolboxDefinition => new ImageDefinition
        {
            Type = ImageType.Xaml,
            Monkier = Monikers.Toolbox,
            Source = UriCreator.Create("Toolbox_16x", ImageType.Xaml)
        };

        public static ImageDefinition Win32TextDefinition => new ImageDefinition
        {
            Type = ImageType.Png,
            Monkier = Monikers.Win32Text,
            Source = UriCreator.Create("Win32TextFile", ImageType.Xaml)
        };
    }
}
