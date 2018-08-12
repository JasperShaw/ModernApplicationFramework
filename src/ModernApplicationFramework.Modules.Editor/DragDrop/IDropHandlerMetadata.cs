using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.DragDrop
{
    public interface IDropHandlerMetadata : IOrderable
    {
        IEnumerable<string> DropFormats { get; }

        [DefaultValue(null)]
        IEnumerable<string> ContentTypes { get; }
    }
}