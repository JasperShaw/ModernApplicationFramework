using System;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Core.Events
{
    public class ItemDoubleClickedEventArgs : EventArgs
    {
        public IExtensionDefinition Extension { get;}

        public ItemDoubleClickedEventArgs(IExtensionDefinition extension)
        {
            Extension = extension;
        }
    }
}
