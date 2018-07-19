using System;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    /// <summary>
    /// Data container for toolbox items.
    /// </summary>
    [Serializable]
    public sealed class ToolboxItemData
    {
        /// <summary>
        /// The data object.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// The data format.
        /// </summary>
        public string Format { get; }

        public ToolboxItemData(string format, object data)
        {
            Format = format;
            Data = data;
        }
    }
}