using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.ImageCatalog;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Modules.Toolbox;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    [Export(typeof(ToolboxItemDefinitionBase))]
    public class AddToolboxItemDefinition : ToolboxItemDefinitionBase
    {
        public override TypeArray<ILayoutItem> CompatibleTypes =>
            new TypeArray<ILayoutItem>(new[] {typeof(GraphViewModel)}, true);

        public override ToolboxItemData Data => new ToolboxItemData(ToolboxItemDataFormats.Type, typeof(Add));

        public override Guid Id => new Guid("{39CD7B9B-27C9-4FD2-ADCF-D6A6BA5165DB}");

        public override string Name => "Add";

        public override ImageMoniker ImageMoniker => Monikers.Add;
    }
}