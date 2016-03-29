using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Controls;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Document
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string DisplayName
        {
            get { return "Sample Browser"; }
        }

        [ImportingConstructor]
        public SampleViewModel([Import] IDockingHostViewModel shell)
        {
            _dockingHostViewModel = shell;
        }

        public override bool ShouldReopenOnStart => true;
    }
}
