using System;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxCategoryDefinition
    {
        public string Name { get; }

        public Guid Id { get; }

        public bool IsVisible { get; set; } = true;

        public ToolboxCategoryDefinition(Guid id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
