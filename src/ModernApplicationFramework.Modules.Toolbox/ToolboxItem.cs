using System;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItem
    {
        public Type LayoutItemType { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public Uri IconSource { get; set; }

        public Type ItemType { get; set; }
    }
}
