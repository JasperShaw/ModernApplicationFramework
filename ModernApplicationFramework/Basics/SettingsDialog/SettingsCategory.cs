using System.Collections.Generic;

namespace ModernApplicationFramework.Basics.SettingsDialog
{
    public class SettingsCategory
    {
        public SettingsCategory Parent { get; }

        public string Name { get; }

        public uint SortOrder { get; }

        public SettingsCategory Root { get; }

        public IList<SettingsCategory> Children { get; }

        public SettingsCategory(string name, uint sortOrder, SettingsCategory parent)
        {
            SortOrder = sortOrder;
            Name = name;
            Parent = parent;
            Children = new List<SettingsCategory>();

            if (parent == null)
                return;

            Parent.Children.Add(this);
            Root = parent.Root;
        }

        public SettingsCategory(string name, uint sortOrder) : this(name, sortOrder,null)
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