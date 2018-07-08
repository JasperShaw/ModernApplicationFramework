using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Imaging;

namespace ModernApplicationFramework.ImageCatalog
{
    [Export(typeof(IImageCatalog))]
    public class FrameworkImageCatalog : IImageCatalog
    {
        public static Guid FrameworkImageCatalogGuid = new Guid("{8282D95C-62FB-46D0-813E-12B417B5BDF0}");

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
                default:
                    return false;
            }           
        }
    }
}
