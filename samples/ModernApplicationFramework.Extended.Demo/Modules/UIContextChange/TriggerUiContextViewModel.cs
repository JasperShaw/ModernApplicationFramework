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

        private static readonly Lazy<UiContext> _testContext = new Lazy<UiContext>(() => UiContext.FromUiContextGuid(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}")));

        private static readonly Lazy<UiContext> _testContext2 = new Lazy<UiContext>(() => UiContext.FromUiContextGuid(new Guid("{4C800FF2-6D1D-45DE-80A2-9ADE26037208}")));

        public static UiContext TestContext => _testContext.Value;

        public static UiContext TestContext2 => _testContext2.Value;


        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand ActivateContextCommand => new DelegateCommand(ExecuteActivate);

        public ICommand DeactivateContextCommand => new DelegateCommand(ExecuteDeactivate);

        public ICommand ActivateContext2Command => new DelegateCommand(ExecuteActivate2);

        public ICommand DeactivateContext2Command => new DelegateCommand(ExecuteDeactivate2);

        private void ExecuteActivate(object obj)
        {
            UiContext.FromUiContextGuid(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}"));
            var m = IoC.Get<IUiContextManager>();
            m.GetUiContextCookie(new Guid("{CA2D40CF-F606-4FE6-ABEB-5B3E07839C55}"), out var cookie);
            m.SetUiContext(cookie, true);

            //TestContext.IsActive = true;
        }

        private void ExecuteDeactivate(object obj)
        {
            TestContext.IsActive = false;
        }

        private void ExecuteActivate2(object obj)
        {
            TestContext2.IsActive = true;
        }

        private void ExecuteDeactivate2(object obj)
        {
            TestContext2.IsActive = false;
        }
    }
}
