using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar.Models;
using ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string DisplayName => "Sample Browser";

        public ICommand ShowComboValueCommand => new Command(ShowComboValue);
        public ICommand AddComboValueCommand => new Command(AddComboValue);

        private void AddComboValue()
        {
            var i = new ComboBoxItemModel("012");
            ComboDataModel.AddItem(i);
            ComboDataModel.SelectedItem = i;
        }

        private void ShowComboValue()
        {
        }

        public ISample[] Samples { get; }


        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;
            ComboDataModel = TestComboModel.Instance;
        }

        public ComboBoxModel ComboDataModel { get; }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            sample.Activate(_dockingHostViewModel);
        }
    }
}
