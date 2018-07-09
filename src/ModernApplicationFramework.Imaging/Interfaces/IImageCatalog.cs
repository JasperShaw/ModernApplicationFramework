using System;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Imaging.Interfaces
{
    public interface IImageCatalog
    {
        Guid ImageCataloGuid { get; }

        bool GetDefinition(int id, out ImageDefinition imageDefinition);
    }
}