using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MoveAllToPreviousTabGroupCommandDefinition))]
    public sealed class MoveAllToPreviousTabGroupCommandDefinition : CommandDefinition<IMoveAllToPreviousTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveAllToPreviousTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveAllToPreviousTabGroupCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{CB3727C1-BD07-4137-BA32-5DB0D55414B4}");
    }
}