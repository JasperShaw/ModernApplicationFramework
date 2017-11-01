using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.WindowManagement
{
    /// <summary>
    /// Options to store and load layout configurations 
    /// </summary>
    [Flags]
    public enum ProcessStateOption
    {
        /// <summary>
        /// Loads/Stores all <see cref="ILayoutItemBase"/> components
        /// </summary>
        Complete = 1,
        /// <summary>
        /// Loads/Stores only <see cref="ITool"/>s
        /// </summary>
        ToolsOnly = 2,
        /// <summary>
        /// Loads/Stores only <see cref="ILayoutItem"/>s
        /// </summary>
        DocumentsOnly = 4,
        /// <summary>
        /// Includes the <see cref="ILayoutItemBase.ShouldReopenOnStart"/> property
        /// </summary>
        UseShouldReopenOnStart = 8
    }
}