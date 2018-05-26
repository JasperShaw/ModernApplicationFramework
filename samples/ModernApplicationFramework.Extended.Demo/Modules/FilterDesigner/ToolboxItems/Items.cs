using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Modules.Toolbox;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal  static ToolboxItem AddItem = new ToolboxItem(typeof(Add),Categories.MathCategory, "Add", new BitmapImage(new Uri("pack://application:,,,/Resources/action_add_16xLG.png")));

        [Export] internal  static ToolboxItem ImageSourceItem = new ToolboxItem(typeof(ImageSource), Categories.GeneratorCategory, "Image Source");
    }
}
