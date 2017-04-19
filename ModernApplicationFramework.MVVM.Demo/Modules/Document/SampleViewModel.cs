using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Extended.Core.LayoutItems.LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;
        private ComboBoxDataSource _dataSource;

        public override string DisplayName => "Sample Browser";

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

        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;
            if (shell == null)
                MessageBox.Show("null");


            DataSource = new ComboBoxDataSource
            {
                Items = new List<object>
                {
                    "Test",
                    "Test1",
                    "Test2"
                },
                SelectedIndex = 1
            };
            DataSource.DisplayedItem = DataSource.Items.FirstOrDefault();
        }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            _dockingHostViewModel.OpenDocument(IoC.Get<UndoRedoViewModel>());
            //_dockingHostViewModel.ShowTool<IOutput>();
        }
    }
}
