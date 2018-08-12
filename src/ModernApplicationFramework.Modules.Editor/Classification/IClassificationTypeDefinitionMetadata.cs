using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    public interface IClassificationTypeDefinitionMetadata
    {
        [DefaultValue(null)] IEnumerable<string> BaseDefinition { get; }

        string Name { get; }
    }
}