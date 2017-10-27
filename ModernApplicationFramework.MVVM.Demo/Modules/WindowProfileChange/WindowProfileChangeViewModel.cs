using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using MordernApplicationFramework.WindowManagement;
using MordernApplicationFramework.WindowManagement.WindowProfile;

namespace ModernApplicationFramework.MVVM.Demo.Modules.WindowProfileChange
{
    [DisplayName("WindowProfile")]
    [Export(typeof(WindowProfileChangeViewModel))]
    public sealed class WindowProfileChangeViewModel: Extended.Core.LayoutItems.LayoutItem
    {
        private readonly IStatusBarDataModelService _statusBar;
        public ICommand SetDefaultCommand => new Command(SetDefault);

        public ICommand SetDesignCommand => new Command(SetDesign);

        public ICommand SetDebugCommand => new Command(SetDebug);

        [ImportingConstructor]
        public WindowProfileChangeViewModel(IStatusBarDataModelService statusBar)
        {
            _statusBar = statusBar;
            DisplayName = "Window Profile Change";
        }

        private void SetDefault()
        {
            LayoutManagementService.Instance.LoadLayout("Default");
            _statusBar.SetText(1, "Default");
        }

        private void SetDesign()
        {
            LayoutManagementService.Instance.LoadLayout("Design");
            _statusBar.SetText(1, "Design");
        }

        private void SetDebug()
        {
            LayoutManagementService.Instance.LoadLayout("Debug");
            _statusBar.SetText(1, "Debug");
        }
    }
}
