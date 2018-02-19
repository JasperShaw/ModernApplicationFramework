using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDialogItemPresenter<out T> : IScreen
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        ObservableCollection<IHasTextProperty> SortItems { get; set; }

        ComboBoxDataSource SortDataSource { get; set; }

        T CreateResult(string name, string path);

        IExtensionDefinition SelectedItem { get; set; }

        int SelectedIndex { get; set; }
    }
}