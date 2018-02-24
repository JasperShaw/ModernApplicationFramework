using System;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        FileType FileType { get; }

        Type PreferredEditor { get; }     
    }
}