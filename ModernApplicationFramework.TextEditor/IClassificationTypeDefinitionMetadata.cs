using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationTypeDefinitionMetadata
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition { get; }
    }
}