using System;
using System.Collections.Generic;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface ISupportedFileDefinition : IExtensionDefinition
    {
        string FileExtension { get; }

        IEnumerable<IFileDefinitionContext> FileContexts { get; }

        Guid PreferredEditor { get; }

        SupportedFileOperation SupportedFileOperation { get; }
    }
}