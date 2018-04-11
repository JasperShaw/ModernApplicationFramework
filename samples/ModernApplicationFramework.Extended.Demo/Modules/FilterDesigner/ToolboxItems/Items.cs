using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal  static IToolboxItem AddItem = new ToolboxItemEx(Categories.MathCategory, "Add");

        [Export] internal  static IToolboxItem ImageSourceItem = new ToolboxItemEx(Categories.GeneratorCategory, "Image Source");
    }
}
