using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Modules.Toolbox;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal  static IToolboxItem AddItem = new ToolboxItemEx(typeof(Add),Categories.MathCategory, "Add");

        [Export] internal  static IToolboxItem ImageSourceItem = new ToolboxItemEx(typeof(ImageSource), Categories.GeneratorCategory, "Image Source");
    }
}
