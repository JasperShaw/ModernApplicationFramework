using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Modules.Toolbox;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Categories
    {
        [Export] internal static ToolboxItemCategory MathCategory =
            new ToolboxItemCategory(typeof(GraphViewModel) ,"Maths");

        [Export]
        internal static ToolboxItemCategory GeneratorCategory =
            new ToolboxItemCategory(typeof(GraphViewModel), "Generators");
    }
}
