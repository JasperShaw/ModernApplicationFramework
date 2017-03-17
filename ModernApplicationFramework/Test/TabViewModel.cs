using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Test
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(TabViewModel))]
    public sealed class TabViewModel: Screen
    {

        private readonly ToolbarDefinition[] _toolbarDefinitions;
        private ToolbarDefinition _selectedToolbarDefinition;


        public ObservableCollection<ToolbarDefinition> Toolbars { get; set; }


        public ToolbarDefinition SelectedToolbarDefinition
        {
            get => _selectedToolbarDefinition;
            set
            {
                if (_selectedToolbarDefinition == value)
                    return;
                if (value == null)
                    return;
                _selectedToolbarDefinition = value;
                NotifyOfPropertyChange();
            }
        }


        [ImportingConstructor]
        public TabViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            DisplayName = "Toolbars";
            _toolbarDefinitions = toolbarDefinitions;

            Toolbars = new ObservableCollection<ToolbarDefinition>();

            foreach (var definition in _toolbarDefinitions)
            {
                Toolbars.Add(definition);
            }
        }
    }
}
