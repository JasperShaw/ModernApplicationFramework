using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IFileToContentTypeMetadata
    {
        [DefaultValue(null)]
        string FileExtension { get; }

        [DefaultValue(null)]
        string FileName { get; }

        IEnumerable<string> ContentTypes { get; }
    }
}