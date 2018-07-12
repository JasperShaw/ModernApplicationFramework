using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ToolboxItems
{
    public static class Categories
    {

        [Export]
        internal static ToolboxCategoryDefinition MathCategoryDefinition =>
            new ToolboxCategoryDefinition(new Guid("{85EA60AA-2F0D-4B3F-AF86-D3C4BD7BCCFE}"), "Maths");

        [Export]
        internal static ToolboxCategoryDefinition GeneratorCategoryDefinition =>
            new ToolboxCategoryDefinition(new Guid("{70247703-6A61-49E4-B870-F79DC7B469F8}"), "Generators");




        [Export]
        internal static IToolboxCategory MathCategory =
            new ToolboxCategory(new Guid("{9D6E52CE-32C4-4EFD-8D02-C986C57765F4}"), MathCategoryDefinition);

        [Export]
        internal static IToolboxCategory GeneratorCategory =
            new ToolboxCategory(new Guid("{2CDE6161-F239-4D6E-8A01-AD524575843D}"), GeneratorCategoryDefinition);
    }
}
