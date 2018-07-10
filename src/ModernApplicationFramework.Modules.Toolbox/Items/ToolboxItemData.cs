using System;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    [Serializable]
    public sealed class ToolboxItemData
    {
        public string Format { get; }

        public object Data { get; }

        public ToolboxItemData(string format, object data)
        {
            Format = format;
            Data = data;
        }
    }
}