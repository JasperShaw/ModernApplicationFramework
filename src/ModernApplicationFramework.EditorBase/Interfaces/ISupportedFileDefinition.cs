using System;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {      
        string FileExtension { get; }

        Guid PreferredEditor { get; }    
        
        SupportedFileOperation SupportedFileOperation { get; }
    }
}