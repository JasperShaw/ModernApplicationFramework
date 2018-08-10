using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
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