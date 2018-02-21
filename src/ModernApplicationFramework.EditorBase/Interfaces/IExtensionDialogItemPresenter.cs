using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDialogItemPresenter<out T> : IScreen, IItemDoubleClickable
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        bool CanOpenWith { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        ObservableCollection<IHasTextProperty> SortItems { get; set; }

        ComboBoxDataSource SortDataSource { get; set; }

        T CreateResult(string name, string path);

        IExtensionDefinition SelectedItem { get; set; }

        int SelectedIndex { get; set; }
    }
}