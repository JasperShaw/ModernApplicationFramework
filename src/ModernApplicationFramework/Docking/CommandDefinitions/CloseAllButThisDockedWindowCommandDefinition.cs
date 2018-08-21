using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseAllButThisDockedWindowCommandDefinition))]
    public sealed class CloseAllButThisDockedWindowCommandDefinition : CommandDefinition<ICloseAllButThisDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseAllButThisDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseAllButThisDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{D323EFFE-7A78-40FB-A1A8-393B4010D848}");
    }
}