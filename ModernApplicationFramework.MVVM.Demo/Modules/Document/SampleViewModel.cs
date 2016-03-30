using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.MVVM.Demo.Modules.Tool;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Controls.Document
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string DisplayName => "Sample Browser";

        public ISample[] Samples { get; }

        [ImportingConstructor]
        public SampleViewModel([Import] IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;
            if (shell == null)
                MessageBox.Show("null");
            shell?.ShowTool<IOutput>();
        }

        public override bool ShouldReopenOnStart => true;
    }
}
