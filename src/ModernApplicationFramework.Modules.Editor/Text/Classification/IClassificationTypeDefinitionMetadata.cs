using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Text.Classification
{
    public interface IClassificationTypeDefinitionMetadata
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition { get; }
    }
}