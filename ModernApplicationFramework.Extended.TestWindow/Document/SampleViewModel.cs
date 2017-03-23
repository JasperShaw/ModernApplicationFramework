using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.TestWindow.UndoRedoTest;

namespace ModernApplicationFramework.Extended.TestWindow.Document
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Core.LayoutItems.LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string DisplayName => "Sample Browser";

        public ISample[] Samples { get; }

        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;
            if (shell == null)
                MessageBox.Show("null");
        }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            _dockingHostViewModel.OpenDocument(IoC.Get<UndoRedoViewModel>());
        }
    }
}
