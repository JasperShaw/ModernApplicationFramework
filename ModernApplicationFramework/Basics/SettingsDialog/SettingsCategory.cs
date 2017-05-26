using System.Collections.Generic;

namespace ModernApplicationFramework.Basics.SettingsDialog
{
    public class SettingsCategory
    {
        public SettingsCategory Parent { get; }

        public string Name { get; }

        public string Text { get; }

        public uint SortOrder { get; }

        public SettingsCategory Root { get; }

        public IList<SettingsCategory> Children { get; }

        public bool IsToolsOptionsCategory { get; } = true;

        public SettingsCategory(string name, string text, uint sortOrder, SettingsCategory parent, bool isToolsOptionsCategory = true)
        {
            SortOrder = sortOrder;
            Name = name;
            Text = text;
            Parent = parent;
            Children = new List<SettingsCategory>();

            if (parent == null)
                return;

            Parent.Children.Add(this);
            Root = parent.Root;
            IsToolsOptionsCategory = isToolsOptionsCategory;
        }

        public SettingsCategory(string name, string text, uint sortOrder, bool isToolsOptionsCategory = true) 
            : this(name, text, sortOrder,null, isToolsOptionsCategory)
        {
            Root = this;
        }

        public IEnumerable<SettingsCategory> Path 
        {
            get
            {
                var path = new LinkedList<SettingsCategory>();
                path.AddFirst(this);
                var c = this;
                while (c.Parent != null)
                {
                    path.AddFirst(c.Parent);
                    c = c.Parent;
                }
                return path;
            }
        }
    }
}