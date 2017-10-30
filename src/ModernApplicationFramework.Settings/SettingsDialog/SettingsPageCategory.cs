using System.Collections.Generic;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    public sealed class SettingsPageCategory
    {
        public SettingsPageCategory Parent { get; }
        
        public string Text { get; }
        
        public uint SortOrder { get; }

        public IList<SettingsPageCategory> Children { get; }
        
        public SettingsPageCategory Root { get; }

        public IEnumerable<SettingsPageCategory> Path
        {
            get
            {
                var path = new LinkedList<SettingsPageCategory>();
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

        public SettingsPageCategory(string text, uint sortOrder, SettingsPageCategory parent)
        {
            Parent = parent;
            Text = text;
            SortOrder = sortOrder;
            Children = new List<SettingsPageCategory>();
            if (parent == null)
                Root = this;
            else
            {
                Parent.Children.Add(this);
                Root = parent.Root;
            }
        }

        public SettingsPageCategory(string text, uint sortOrder) : this(text, sortOrder, null)
        {
            
        }
    }
}
