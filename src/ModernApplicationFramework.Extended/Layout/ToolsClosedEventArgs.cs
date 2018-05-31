using System;
using System.Collections.Generic;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class ToolsClosedEventArgs : EventArgs
    {
        public IEnumerable<ITool> Tools { get; }

        public ToolsClosedEventArgs(IEnumerable<ITool> tools)
        {
            Tools = tools;
        }
    }
}