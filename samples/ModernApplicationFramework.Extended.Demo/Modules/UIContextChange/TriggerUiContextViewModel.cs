using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.UIContext;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(TriggerUiContextViewModel))]
    public class TriggerUiContextViewModel : Tool
    {

        private static readonly Lazy<UiContext> _testContext = new Lazy<UiContext>(() => UiContext.FromUIContextGuid(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}")));

        public static UiContext TestContext => _testContext.Value;


        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand ActivateContextCommand => new DelegateCommand(ExecuteActivate);

        public ICommand DeactivateContextCommand => new DelegateCommand(ExecuteDeactivate);

        private void ExecuteActivate(object obj)
        {
           UiContext.FromUIContextGuid(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}"));
            var m = IoC.Get<IUiContextManager>();
            m.GetUiContextCookie(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}"), out var cookie);
            m.SetUiContext(cookie, true);
        }

        private void ExecuteDeactivate(object obj)
        {
            
        }
    }
}
