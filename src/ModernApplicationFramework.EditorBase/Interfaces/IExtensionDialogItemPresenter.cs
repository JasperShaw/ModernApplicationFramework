using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDialogItemPresenter : IListViewContainer
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        ObservableCollection<IHasTextProperty> SortItems { get; set; }

        ComboBoxDataSource SortDataSource { get; set; }

        object CreateResult(string name, string path);

        IExtensionDefinition SelectedItem { get; }
    }
}