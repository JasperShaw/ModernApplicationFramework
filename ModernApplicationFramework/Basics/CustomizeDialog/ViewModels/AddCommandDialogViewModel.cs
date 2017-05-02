using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAddCommandDialogViewModel))]
    public sealed class AddCommandDialogViewModel : Screen, IAddCommandDialogViewModel
    {
        private CommandCategory _selectedCategory;
        private IEnumerable<CommandBarItemDefinition> _items;
        private CommandBarItemDefinition _selectedItem;

        public ICommand OkClickCommand => new Command(ExecuteOkClick);

        public IEnumerable<CommandCategory> Categories { get; }

        public IEnumerable<DefinitionBase> AllCommandDefinitions { get; }

        public IEnumerable<CommandBarItemDefinition> Items
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

        public CommandBarItemDefinition SelectedItem
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

        public AddCommandDialogViewModel()
        {
            DisplayName = Customize_Resources.AddCommandPrompt_Title;

            var categories = IoC.GetAll<CommandCategory>();
            Categories = new List<CommandCategory>(categories);
            var allCommandDefinitions = IoC.GetAll<DefinitionBase>();
            AllCommandDefinitions = new List<DefinitionBase>(allCommandDefinitions);
            //Items = new List<CommandBarItemDefinition>();
        }

        public void UpdateItems()
        {
            if (!AllCommandDefinitions.Any())
                return;
	        Items = (from commandDefinition in AllCommandDefinitions
		        where commandDefinition.Category == SelectedCategory
		        select new CommandBarCommandItemDefinition(0, commandDefinition)).ToList(); //Slower to .ToList but actually fixes the CustomSort not beeing used
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var v = view as AddCommandDialogView;
            if (!Categories.Any() || v == null)
                return;
            //This fixes an issue with themed icons
            v.CategoriesListView.SelectedIndex = 0;
            v.CategoriesListView.SelectedIndex = -1;
            v.CategoriesListView.SelectedIndex = 0;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateItems();
        }

        private void ExecuteOkClick()
        {
            TryClose(true);
        }
    }
}