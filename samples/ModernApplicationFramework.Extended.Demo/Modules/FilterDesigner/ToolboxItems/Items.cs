using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal static IToolboxItem AddItem = new ToolboxItem(
            new Guid("{BF8B09BC-2475-4688-9801-5605FF1D6680}"), Categories.MathCategory, new AddToolboxItemDefinition());

        [Export] internal static IToolboxItem ImageSourceItem = new ToolboxItem(
            new Guid("{B8207150-3C51-4606-8BB3-94723CFAC493}"), Categories.GeneratorCategory, new ImageSourceItemDefinition());
    }
}
