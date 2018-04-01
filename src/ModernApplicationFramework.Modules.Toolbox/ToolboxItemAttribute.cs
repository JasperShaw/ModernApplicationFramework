using System;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolboxItemAttribute : Attribute
    {
        public string IconSource { get; }

        public string Category { get; }

        public string Name { get; }

        public Type LayoutItemType { get; }

        public ToolboxItemAttribute(Type layoutItemType, string name, string category, string iconSource = null)
        {
            LayoutItemType = layoutItemType;
            Name = name;
            Category = category;
            IconSource = iconSource;
        }
    }
}
