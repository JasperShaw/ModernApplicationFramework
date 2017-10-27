using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.MVVM.Demo.Modules.WindowProfileChange
{
    [DisplayName("WindowProfile")]
    [Export(typeof(WindowProfileChangeViewModel))]
    public sealed class WindowProfileChangeViewModel: Extended.Core.LayoutItems.LayoutItem
    {
        public ICommand SetDefaultCommand => new Command(SetDefault);

        public ICommand SetDesignCommand => new Command(SetDesign);

        public ICommand SetDebugCommand => new Command(SetDebug);

        [ImportingConstructor]
        public WindowProfileChangeViewModel()
        {
            DisplayName = "Window Profile Change";
        }

        private void SetDefault()
        {
        }

        private void SetDesign()
        {
        }

        private void SetDebug()
        {
        }
    }
}
