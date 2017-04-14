using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAddCommandDialogViewModel))]
    public sealed class AddCommandDialogViewModel : Screen, IAddCommandDialogViewModel
    {
        private CommandCategory _selectedCategory;
        private IEnumerable<CommandBarDefinitionBase> _items;
        private CommandBarDefinitionBase _selectedItem;

        public ICommand OkClickCommand => new Command(ExecuteOkClick);

        public IEnumerable<CommandCategory> Categories { get; }

        public IEnumerable<DefinitionBase> AllCommandDefinitions { get; }

        public IEnumerable<CommandBarDefinitionBase> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange();
            }
        }

        public CommandCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (Equals(value, _selectedCategory))
                    return;
                _selectedCategory = value;
                NotifyOfPropertyChange();
                UpdateItems();
            }
        }

        public CommandBarDefinitionBase SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem))
                    return;
                _selectedItem = value;
                NotifyOfPropertyChange();
            }
        }

        public void UpdateItems()
        {
            if(!AllCommandDefinitions.Any())
                return;
            Items =
                (from commandDefinition in AllCommandDefinitions
                    where commandDefinition.Category == SelectedCategory
                    select new CommandBarCommandItemDefinition(0, commandDefinition)).Cast<CommandBarDefinitionBase>()
                .ToList();
        }

        public AddCommandDialogViewModel()
        {
            DisplayName = "Add Command";
            var categories = IoC.GetAll<CommandCategory>();
            Categories = new List<CommandCategory>(categories);
            var allCommandDefinitions = IoC.GetAll<DefinitionBase>();
            AllCommandDefinitions = new List<DefinitionBase>(allCommandDefinitions);
            Items = new List<CommandBarDefinitionBase>();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var v = view as AddCommandDialogView;
            if (!Categories.Any() || v == null)           
                return;
            v.CategoriesListView.SelectedIndex = 0;
        }

        private void ExecuteOkClick()
        {
            TryClose(true);
        }
    }

    public interface IAddCommandDialogViewModel : IScreen
    {
        ICommand OkClickCommand { get; }

        IEnumerable<CommandCategory> Categories { get; }

        IEnumerable<DefinitionBase> AllCommandDefinitions { get; }

        IEnumerable<CommandBarDefinitionBase> Items { get; set; }

        CommandCategory SelectedCategory { get; set; }

        CommandBarDefinitionBase SelectedItem { get; set; }

        void UpdateItems();
    }
}
