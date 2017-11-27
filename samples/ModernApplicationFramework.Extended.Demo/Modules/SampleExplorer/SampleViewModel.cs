using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.SampleExplorer
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        private ComboBoxDataSource _dataSource;

        public override string DisplayName => "Sample Browser";

        public ICommand ShowComboValueCommand => new Command(ShowComboValue);
        public ICommand AddComboValueCommand => new Command(AddComboValue);

        private void AddComboValue()
        {
            Items.Add(new TextCommandBarItemDefinition("Test 4"));
        }

        private void ShowComboValue()
        {
        }

        public ISample[] Samples { get; }

        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<IHasTextProperty> Items { get; set; }

        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;

            Items = new ObservableCollection<IHasTextProperty>
            {
                new TextCommandBarItemDefinition("Test"),
                new TextCommandBarItemDefinition("Test2"),
                new TextCommandBarItemDefinition("Test3"),
            };

            DataSource = new ComboBoxDataSource(Items);
        }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            sample.Activate(_dockingHostViewModel);
        }
    }
}
