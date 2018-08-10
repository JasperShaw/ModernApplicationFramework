using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IContentTypeMetadata
    {
        IEnumerable<string> ContentTypes { get; }
    }
}