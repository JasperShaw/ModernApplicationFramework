using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseDockedWindowCommandDefinition))]
    public sealed class CloseDockedWindowCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }

        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{3CFF0A7E-3BE7-47FD-B12F-6195773866DE}");

        public CloseDockedWindowCommandDefinition()
        {
            Command = new UICommand(CloseDockedWindow, CanCloseDockedWindow);
        }

        private bool CanCloseDockedWindow()
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            return dm != null;
        }

        private void CloseDockedWindow()
        {
            var dm = DockingManager.Instance?.Layout.ActiveContent;
            var item = DockingManager.Instance?.GetLayoutItemFromModel(dm);
            item?.CloseCommand.Execute(null);
        }
    }
}