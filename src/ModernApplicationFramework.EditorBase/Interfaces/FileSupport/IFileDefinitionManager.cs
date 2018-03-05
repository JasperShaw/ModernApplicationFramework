using System.Collections.Generic;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFileDefinitionManager
    {
        IReadOnlyCollection<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        ISupportedFileDefinition GetDefinitionByExtension(string extension);
    }
}