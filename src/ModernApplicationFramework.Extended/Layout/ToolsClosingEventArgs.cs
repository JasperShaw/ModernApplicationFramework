using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class ToolsClosingEventArgs : CancelEventArgs
    {
        public IEnumerable<ITool> LayoutItems { get; }
        public AnchorableCloseMode Mode { get; }

        public ToolsClosingEventArgs(IEnumerable<ITool> layoutItems, AnchorableCloseMode mode)
        {
            LayoutItems = layoutItems;
            Mode = mode;
        }
    }
}