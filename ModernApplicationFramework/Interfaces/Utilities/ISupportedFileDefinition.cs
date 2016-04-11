using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        FileType FileType { get; }
        Type PrefferedEditor { get; }     
    }
}