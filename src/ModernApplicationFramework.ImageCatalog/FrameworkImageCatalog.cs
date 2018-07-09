using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    [Export(typeof(IImageCatalog))]
    public class FrameworkImageCatalog : IImageCatalog
    {
        public const string Guid = "8282D95C-62FB-46D0-813E-12B417B5BDF0";

        public static Guid FrameworkImageCatalogGuid = new Guid(Guid);

        public Guid ImageCataloGuid => FrameworkImageCatalogGuid;

        public bool GetDefinition(int id, out ImageDefinition imageDefinition)
        {
            imageDefinition = default;
            switch (id)
            {
                case ImageIds.Undo:
                    imageDefinition = ImageDefinitions.UndoDefinition;
                    return true;
                case ImageIds.StatusInfo:
                    imageDefinition = ImageDefinitions.StatusInfoDefinition;
                    return true;
                case ImageIds.StatusWarning:
                    imageDefinition = ImageDefinitions.StatusWarningDefinition;
                    return true;
                case ImageIds.StatusError:
                    imageDefinition = ImageDefinitions.StatusErrorDefinition;
                    return true;
                case ImageIds.CloseDocumentGroup:
                    imageDefinition = ImageDefinitions.CloseDocumentGroupDefinition;
                    return true;
                case ImageIds.HideToolWindow:
                    imageDefinition = ImageDefinitions.HideToolWindowDefinition;
                    return true;
                case ImageIds.SplitScreenHorizontal:
                    imageDefinition = ImageDefinitions.SplitScreenHorizontalDefinition;
                    return true;
                case ImageIds.SplitScreenVertical:
                    imageDefinition = ImageDefinitions.SplitScreenVerticalDefinition;
                    return true;
                case ImageIds.CloseProgram:
                    imageDefinition = ImageDefinitions.CloseProgramDefinition;
                    return true;
                case ImageIds.Copy:
                    imageDefinition = ImageDefinitions.CopyDefinition;
                    return true;
                case ImageIds.Cut:
                    imageDefinition = ImageDefinitions.CutDefinition;
                    return true;
                case ImageIds.Delete:
                    imageDefinition = ImageDefinitions.DeleteDefinition;
                    return true;
                case ImageIds.FitToScreen:
                    imageDefinition = ImageDefinitions.FitToScreenDefinition;
                    return true;
                case ImageIds.MoveDown:
                    imageDefinition = ImageDefinitions.MoveDownDefinition;
                    return true;
                case ImageIds.MoveUp:
                    imageDefinition = ImageDefinitions.MoveUpDefinition;
                    return true;
                case ImageIds.Paste:
                    imageDefinition = ImageDefinitions.PasteDefinition;
                    return true;
                case ImageIds.Redo:
                    imageDefinition = ImageDefinitions.RedoDefinition;
                    return true;
                case ImageIds.Settings:
                    imageDefinition = ImageDefinitions.SettingsDefinition;
                    return true;
                case ImageIds.OpenFolder:
                    imageDefinition = ImageDefinitions.OpenFolderDefinition;
                    return true;
                case ImageIds.Save:
                    imageDefinition = ImageDefinitions.SaveDefinition;
                    return true;
                case ImageIds.SaveAll:
                    imageDefinition = ImageDefinitions.SaveAllDefinition;
                    return true;
                case ImageIds.NewFile:
                    imageDefinition = ImageDefinitions.NewFileDefinition;
                    return true;
                case ImageIds.Property:
                    imageDefinition = ImageDefinitions.PropertyDefinition;
                    return true;
                case ImageIds.Output:
                    imageDefinition = ImageDefinitions.OutputDefinition;
                    return true;
                case ImageIds.Toolbox:
                    imageDefinition = ImageDefinitions.ToolboxDefinition;
                    return true;
                case ImageIds.Win32Text:
                    imageDefinition = ImageDefinitions.Win32TextDefinition;
                    return true;
                default:
                    return false;
            }           
        }
    }
}
