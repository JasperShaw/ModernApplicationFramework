using System;
using System.Collections.Generic;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolboxItemDataAttribute : Attribute
    {
        public IEnumerable<Type> CompatibleTypes { get; }

        public ImageMoniker Moniker { get; }

        public bool Serializable { get; }

        public string Name { get; }


        public ToolboxItemDataAttribute(string name, params Type[] compatibleTypes) : this(name, Guid.Empty.ToString(), 0 , true, compatibleTypes)
        {

        }

        public ToolboxItemDataAttribute(string name, string imageCatalogGuid, int imageId, params Type[] compatibleTypes) : this(name, imageCatalogGuid, imageId, true, compatibleTypes)
        {
            
        }

        public ToolboxItemDataAttribute(string name, string imageCatalogGuid, int imageId, bool serializable = true, params Type[] compatibleTypes)
        {
            Name = name;
            CompatibleTypes = compatibleTypes;
            Moniker = new ImageMoniker{CatalogGuid = new Guid(imageCatalogGuid), Id = imageId};
            Serializable = serializable;
        }
    }
}
