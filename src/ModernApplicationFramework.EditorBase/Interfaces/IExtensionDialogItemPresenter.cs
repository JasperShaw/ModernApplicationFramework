using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDialogItemPresenter : IScreen, IItemDoubleClickable
    {
        event RoutedPropertyChangedEventHandler<object> ProviderSelectionChanged;

        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        bool CanOpenWith { get; }

        bool IsLargeIconsViewButtonVisible { get; }

        bool IsSmallIconsViewButtonVisible { get; }

        bool IsMediumIconsViewButtonVisible { get; }

        INewElementExtensionsProvider SelectedProvider { get; set; }

        ObservableCollection<INewElementExtensionsProvider> Providers { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        ObservableCollection<ISortingComboboxItem> SortItems { get; set; }

        ComboBoxDataSource SortDataSource { get; set; }


        IExtensionDefinition SelectedItem { get; set; }

        int SelectedIndex { get; set; }
    }

    public interface IExtensionDialogItemPresenter<out T> : IExtensionDialogItemPresenter
    {
        T CreateResult(string name, string path);
    }
}