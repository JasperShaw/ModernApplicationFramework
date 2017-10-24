using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ManageLayoutCommandDefinition))]
    public sealed class ManageLayoutCommandDefinition : CommandDefinition
    {
        private readonly ILayoutManager _layoutManager;
        public override string Name => WindowManagement_Resources.ManageLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ManageLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ManageLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ManageLayoutCommandDefinition(ILayoutManager layoutManager)
        {
            _layoutManager = layoutManager;

            var command = new UICommand(Manage, CanManage);
            Command = command;
        }

        private bool CanManage()
        {
            return true;
        }

        private void Manage()
        {
            _layoutManager.ManageWindowLayouts();
        }
    }
}
