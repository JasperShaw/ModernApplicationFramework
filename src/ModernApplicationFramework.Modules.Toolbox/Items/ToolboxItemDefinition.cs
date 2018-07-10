using System;
using System.Collections.Generic;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    [Serializable]
    public class ToolboxItemDefinition : ToolboxItemDefinitionBase
    {
        public override Guid Id { get; }
        public override string Name { get; }
        public override ToolboxItemData Data { get; }
        public override TypeArray<ILayoutItem> CompatibleTypes { get; }
        public override ImageMoniker ImageMoniker { get; }
        public override bool Serializable { get; }

        public ToolboxItemDefinition(Guid id, string name, ToolboxItemData data, IEnumerable<Type> compatibleTypes, ImageMoniker moniker = default, bool serializable = true)
        {
            Id = id;
            Name = name;
            Data = data;
            CompatibleTypes = new TypeArray<ILayoutItem>(compatibleTypes, true);
            ImageMoniker = moniker;
            Serializable = serializable;

        }

        public ToolboxItemDefinition(string name, ToolboxItemData data, IEnumerable<Type> compatibleTypes, ImageMoniker moniker = default, bool serializable = true) :
            this(Guid.Empty, name, data, compatibleTypes, moniker, serializable)
        {          
        }

        public ToolboxItemDefinition(ToolboxItemDefinitionBase dataSource)
        {
            Id = dataSource.Id;
            Name = dataSource.Name;
            CompatibleTypes = dataSource.CompatibleTypes;
            ImageMoniker = dataSource.ImageMoniker;
            Data = dataSource.Data;
            Serializable = dataSource.Serializable;
        }
    }
}