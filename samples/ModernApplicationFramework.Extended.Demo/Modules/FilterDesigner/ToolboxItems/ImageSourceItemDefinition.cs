using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Modules.Toolbox;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    [Export(typeof(ToolboxItemDefinitionBase))]
    public class ImageSourceItemDefinition : ToolboxItemDefinitionBase
    {
        public override ToolboxItemData Data => new ToolboxItemData(ToolboxItemDataFormats.Type, typeof(ImageSource));

        public override TypeArray<ILayoutItem> CompatibleTypes =>
            new TypeArray<ILayoutItem>(new[] { typeof(GraphViewModel) }, true);

        public override Guid Id => new Guid("{DC1DDB4D-0359-48AA-84B5-59E0F0AA7F7C}");

        public override string Name => "Image Source";
    }
}