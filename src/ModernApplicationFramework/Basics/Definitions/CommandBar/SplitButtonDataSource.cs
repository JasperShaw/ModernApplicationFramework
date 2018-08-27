using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Models;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    internal class SplitButtonDataSource : CommandBarItemDataSource
    {
        private int _selectedIndex;
        private string _statusString;
        private SplitButtonModel _model;
        private ObservableCollection<IHasTextProperty> _items;

        public override CommandControlTypes UiType => CommandControlTypes.SplitDropDown;

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                OnPropertyChanged();
                if (StringCreator != null)
                    StatusString = StringCreator.CreateMessage(value + 1);
            }
        }

        public string StatusString
        {
            get => _statusString;
            set
            {
                if (value == _statusString) return;
                _statusString = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IHasTextProperty> Items =>
            _items ?? (_items = Model.Items);


        public SplitButtonModel Model
        {
            get => _model;
            set
            {
                if (Equals(value, _model)) return;
                _model = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Items));
            }
        }

        public IStatusStringCreator StringCreator { get; }


        public SplitButtonDataSource(Guid id, string text, uint sortOrder, CommandBarGroup group, CommandSplitButtonDefinition definition, 
            bool isCustom, CommandBarFlags flags = CommandBarFlags.CommandFlagNone) 
            : base(text, sortOrder, group, definition, isCustom, false, flags)
        {
            if (definition == null)
                throw new ArgumentNullException();
            Model = definition.Model;
            Id = id;        
            StringCreator = Model.StatusStringCreator;
            StatusString = StringCreator?.CreateDefaultMessage();
        }

        public override Guid Id { get; }
    }
}