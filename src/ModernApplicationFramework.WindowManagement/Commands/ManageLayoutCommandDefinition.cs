using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ManageLayoutCommandDefinition))]
    public sealed class ManageLayoutCommandDefinition : CommandDefinition
    {
        public override string Name => WindowManagement_Resources.ManageLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ManageLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ManageLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{F843A14E-3840-4CC9-AA3F-D70B04DD0ED3}");

        public override ICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        [ImportingConstructor]
        public ManageLayoutCommandDefinition()
        {
            var command = new UICommand(Manage, CanManage);
            Command = command;
        }

        private bool CanManage()
        {
            if (LayoutManagementService.Instance == null)
                return false;
            return true;
        }

        private void Manage()
        {
            LayoutManagementService.Instance.LayoutManager.ManageWindowLayouts();
        }
    }
}
