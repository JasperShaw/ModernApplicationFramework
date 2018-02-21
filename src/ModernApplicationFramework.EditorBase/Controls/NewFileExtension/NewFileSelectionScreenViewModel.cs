using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.EditorBase.Controls.NewElementDialog;
using ModernApplicationFramework.EditorBase.Core;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.NewFileExtension
{
    [Export(typeof(NewFileSelectionScreenViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileCommandArguments>
    {
        public override bool UsesNameProperty => false;

        public override bool UsesPathProperty => false;
        public override string NoItemsMessage => "Not file templates found";
        public override string NoItemSelectedMessage => "No item selected";

        public override NewFileCommandArguments CreateResult(string name, string path)
        {
            return !(SelectedItem is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PreferredEditor);
        }
    }

    public abstract class NewElementScreenViewModelBase<T> : Screen, IExtensionDialogItemPresenter<T>
    {
        private int _selectedIndex;
        private IExtensionDefinition _selectedItem;
        private ComboBoxDataSource _sortDataSource;
        private IEnumerable<IExtensionDefinition> _itemSource;

        public abstract bool UsesNameProperty { get; }

        public abstract bool UsesPathProperty { get; }

        public abstract string NoItemsMessage { get; }

        public abstract string NoItemSelectedMessage { get; }

        public IEnumerable<IExtensionDefinition> ItemSource
        {
            get => _itemSource;
            set
            {
                if (Equals(value, _itemSource)) return;
                _itemSource = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<IHasTextProperty> SortItems { get; set; }

        public ComboBoxDataSource SortDataSource
        {
            get => _sortDataSource;
            set
            {
                if (Equals(value, _sortDataSource)) return;
                _sortDataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public IExtensionDefinition SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                NotifyOfPropertyChange();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex) return;
                _selectedIndex = value;
                NotifyOfPropertyChange();
            }
        }

        protected NewElementScreenViewModelBase()
        {
            ViewModelBinder.Bind(this, new NewElementPresenterView(), null);

            SortItems = new ObservableCollection<IHasTextProperty>
            {
                new TextCommandBarItemDefinition("Test"),
                new TextCommandBarItemDefinition("Test2"),
                new TextCommandBarItemDefinition("Test3"),
            };
            SortDataSource = new ComboBoxDataSource(SortItems);
            SortDataSource.ChangeDisplayedItem(SortDataSource.Items.Count - 1);
        }

        public abstract T CreateResult(string name, string path);
    }
}
