using System.Collections.Generic;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IFileDefinitionManager
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        ISupportedFileDefinition GetDefinitionByExtension(string extension);
    }
}