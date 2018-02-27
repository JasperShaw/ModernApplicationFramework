using System;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        string FileExtension { get; }

        Type PreferredEditor { get; }    
        
        SupportedFileOperation SupportedFileOperation { get; }
    }
}