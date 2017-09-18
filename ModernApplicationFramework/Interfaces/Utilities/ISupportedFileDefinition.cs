using System;
using ModernApplicationFramework.Basics;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        FileType FileType { get; }
        Type PreferredEditor { get; }     
    }
}