using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.WindowManagement;

namespace ModernApplicationFramework.Extended.Demo.Modules.WindowProfileChange
{
    [DisplayName("WindowProfile")]
    [Export(typeof(WindowProfileChangeViewModel))]
    public sealed class WindowProfileChangeViewModel: LayoutItem
    {
        private readonly IStatusBarDataModelService _statusBar;
        public ICommand SetDefaultCommand => new Command(SetDefault, CanSet);

        public ICommand SetDesignCommand => new Command(SetDesign, CanSet);

        public ICommand SetDebugCommand => new Command(SetDebug, CanSet);

        [ImportingConstructor]
        public WindowProfileChangeViewModel(IStatusBarDataModelService statusBar)
        {
            _statusBar = statusBar;
            DisplayName = "Window Profile Change";
        }

        private void SetDefault()
        {
            LayoutManagementPackage.Instance.LayoutManagementSystem.LoadOrCreateProfile("Default");
            _statusBar.SetText(1, "Default");
        }

        private void SetDesign()
        {
            LayoutManagementPackage.Instance.LayoutManagementSystem.LoadOrCreateProfile("Design");
            _statusBar.SetText(1, "Design");
        }

        private void SetDebug()
        {
            LayoutManagementPackage.Instance.LayoutManagementSystem.LoadOrCreateProfile("Debug");
            _statusBar.SetText(1, "Debug");
        }

        private static bool CanSet()
        {
            return LayoutManagementPackage.Instance?.Initialized != null;
        }
    }
}
