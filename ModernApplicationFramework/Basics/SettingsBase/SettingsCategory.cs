using System.Collections.Generic;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Basics.SettingsBase
{
    public class SettingsCategory : ISettingsCategory
    {
        public ISettingsCategory Parent { get; }

        public string Name { get; }

        public string Text { get; }

        public uint SortOrder { get; }

        public ISettingsCategory Root { get; }

        public IList<ISettingsCategory> Children { get; }

        public bool IsToolsOptionsCategory { get; } = true;

        public SettingsCategory(string name, string text, uint sortOrder, ISettingsCategory parent, bool isToolsOptionsCategory = true)
        {
            SortOrder = sortOrder;
            Name = name;
            Text = text;
            Parent = parent;
            Children = new List<ISettingsCategory>();

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

        public IEnumerable<ISettingsCategory> Path 
        {
            get
            {
                var path = new LinkedList<ISettingsCategory>();
                path.AddFirst(this);
                var c = this as ISettingsCategory;
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