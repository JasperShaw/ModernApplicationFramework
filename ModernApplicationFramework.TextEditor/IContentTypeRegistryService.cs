using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface IContentTypeRegistryService
    {
        IContentType GetContentType(string typeName);

        IContentType AddContentType(string typeName, IEnumerable<string> baseTypeNames);

        void RemoveContentType(string typeName);

        IContentType UnknownContentType { get; }

        IEnumerable<IContentType> ContentTypes { get; }
    }
}