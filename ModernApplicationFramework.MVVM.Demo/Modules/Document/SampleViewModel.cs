using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Extended.Core.LayoutItems.LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        private ComboBoxDataSource _dataSource;
        private object _comboValue;

        public override string DisplayName => "Sample Browser";

        public ICommand ShowComboValueCommand => new Command(ShowComboValue);
        public ICommand AddComboValueCommand => new Command(AddComboValue);

        private void AddComboValue()
        {
            Items.Add(new TestItem("Test 4"));
        }

        private void ShowComboValue()
        {
            MessageBox.Show(DataSource.DisplayedItem.ToString());
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

        public ObservableCollection<TestItem> Items { get; set; }

        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;

            Items = new ObservableCollection<TestItem>
            {
                new TestItem("Test"),
                new TestItem("Test2"),
                new TestItem("Test3"),
            };

            DataSource = new ComboBoxDataSource(Items);
        }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            _dockingHostViewModel.OpenDocument(IoC.Get<UndoRedoViewModel>());
        }
    }

    public class TestItem
    {
        public TestItem(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
