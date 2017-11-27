using System;
using ModernApplicationFramework.Basics;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        FileType FileType { get; }
        Type PreferredEditor { get; }     
    }
}