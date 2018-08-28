using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(CloseDockedWindowCommandDefinition))]
    public sealed class CloseDockedWindowCommandDefinition : CommandDefinition<ICloseDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandCategories.FileCategory;
        public override Guid Id => new Guid("{3CFF0A7E-3BE7-47FD-B12F-6195773866DE}");
    }
}