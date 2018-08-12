using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IContentTypeDefinitionMetadata
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> BaseDefinition { get; }

        [DefaultValue(null)]
        string MimeType { get; }
    }
}