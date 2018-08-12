using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities.Core
{
    public interface IContentTypeMetadata
    {
        IEnumerable<string> ContentTypes { get; }
    }
}