using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels;
using ModernApplicationFramework.Modules.Toolbox;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Categories
    {
        [Export]
        internal static IToolboxCategory MathCategory =
            new ToolboxItemCategory(new Guid("{9D6E52CE-32C4-4EFD-8D02-C986C57765F4}"), typeof(GraphViewModel), "Maths");

        [Export]
        internal static IToolboxCategory GeneratorCategory =
            new ToolboxItemCategory(new Guid("{2CDE6161-F239-4D6E-8A01-AD524575843D}"), typeof(GraphViewModel), "Generators");
    }
}
