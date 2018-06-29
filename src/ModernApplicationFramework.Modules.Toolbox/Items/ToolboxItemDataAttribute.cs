using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolboxItemDataAttribute : Attribute
    {
        public IEnumerable<Type> CompatibleTypes { get; }

        public BitmapSource IconSource { get; }

        public bool Serializable { get; }

        public string Name { get; }


        public ToolboxItemDataAttribute(string name, params Type[] compatibleTypes) : this(name, null, true, compatibleTypes)
        {

        }

        public ToolboxItemDataAttribute(string name, string uri, params Type[] compatibleTypes) : this(name, uri, true, compatibleTypes)
        {
            
        }

        public ToolboxItemDataAttribute(string name, string uri, bool serializable = true, params Type[] compatibleTypes)
        {
            Name = name;
            CompatibleTypes = compatibleTypes;
            IconSource = new BitmapImage(new Uri(uri));
            Serializable = serializable;
        }
    }
}
