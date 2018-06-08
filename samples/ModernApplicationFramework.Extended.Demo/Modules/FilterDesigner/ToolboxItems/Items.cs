using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Items
    {
        [Export] internal  static IToolboxItem AddItem = new ToolboxItem(new Guid("{BF8B09BC-2475-4688-9801-5605FF1D6680}") , "Add", typeof(Add), Categories.MathCategory, new []{typeof(GraphViewModel)} ,new BitmapImage(new Uri("pack://application:,,,/Resources/action_add_16xLG.png")));

        [Export] internal  static IToolboxItem ImageSourceItem = new ToolboxItem(new Guid("{B8207150-3C51-4606-8BB3-94723CFAC493}"), "Image Source", typeof(ImageSource), Categories.GeneratorCategory, new[] { typeof(GraphViewModel) });
    }
}
