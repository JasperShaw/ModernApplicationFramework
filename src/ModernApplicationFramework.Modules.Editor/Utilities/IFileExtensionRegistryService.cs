using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IFileExtensionRegistryService
    {
        IContentType GetContentTypeForExtension(string extension);

        IEnumerable<string> GetExtensionsForContentType(IContentType contentType);

        void AddFileExtension(string extension, IContentType contentType);

        void RemoveFileExtension(string extension);

        IContentType GetContentTypeForFileName(string name);

        IContentType GetContentTypeForFileNameOrExtension(string name);

        IEnumerable<string> GetFileNamesForContentType(IContentType contentType);

        void AddFileName(string name, IContentType contentType);

        void RemoveFileName(string name);
    }
}