using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Modules.Toolbox;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal  static IToolboxItem AddItem = new ToolboxItemEx("Add", typeof(Add), Categories.MathCategory, new BitmapImage(new Uri("pack://application:,,,/Resources/action_add_16xLG.png")));

        [Export] internal  static IToolboxItem ImageSourceItem = new ToolboxItemEx("Image Source", typeof(ImageSource), Categories.GeneratorCategory);
    }
}
