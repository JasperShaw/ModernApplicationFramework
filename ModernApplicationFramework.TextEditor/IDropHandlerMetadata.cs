using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    public interface IDropHandlerMetadata : IOrderable
    {
        IEnumerable<string> DropFormats { get; }

        [DefaultValue(null)]
        IEnumerable<string> ContentTypes { get; }
    }
}