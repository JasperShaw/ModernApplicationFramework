using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(CloseAllButThisDockedWindowCommandDefinition))]
    public sealed class CloseAllButThisDockedWindowCommandDefinition : CommandDefinition<ICloseAllButThisDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseAllButThisDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseAllButThisDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandBarCategory Category => CommandBarCategories.FileCategory;
        public override Guid Id => new Guid("{D323EFFE-7A78-40FB-A1A8-393B4010D848}");
    }
}