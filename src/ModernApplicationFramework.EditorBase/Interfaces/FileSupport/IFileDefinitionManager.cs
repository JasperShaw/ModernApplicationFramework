using System.Collections.Generic;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFileDefinitionManager
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        ISupportedFileDefinition GetDefinitionByExtension(string extension);
    }
}