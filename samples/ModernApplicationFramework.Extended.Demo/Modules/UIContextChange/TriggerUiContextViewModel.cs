using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(TriggerUiContextViewModel))]
    public class TriggerUiContextViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand ActivateContextCommand => new DelegateCommand(ExecuteActivate);

        public ICommand DeactivateContextCommand => new DelegateCommand(ExecuteDeactivate);

        private void ExecuteActivate(object obj)
        {
            var a = UIContext.UiContext.FromUIContextGuid(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}")).IsActive;
        }

        private void ExecuteDeactivate(object obj)
        {
            
        }
    }
}
