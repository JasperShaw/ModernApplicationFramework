using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface IExtensionDialogItemPresenter : IScreen, IItemDoubleClickable
    {
        event RoutedPropertyChangedEventHandler<object> ProviderSelectionChanged;
        event RoutedPropertyChangedEventHandler<object> CategorySelectionChanged;

        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        bool CanOpenWith { get; }

        bool IsLargeIconsViewButtonVisible { get; }

        bool IsSmallIconsViewButtonVisible { get; }

        bool IsMediumIconsViewButtonVisible { get; }

        INewElementExtensionsProvider SelectedProvider { get; set; }

        ObservableCollection<INewElementExtensionsProvider> Providers { get; }

        bool ProvidersUsed { get; }

        IEnumerable<IExtensionDefinition> Extensions { get; set; }

        ObservableCollection<ISortingComboboxItem> SortItems { get; set; }

        ComboBoxDataSource SortDataSource { get; set; }


        IExtensionDefinition SelectedExtension { get; set; }

        int SelectedExtensionIndex { get; set; }


        object SelectedProviderTreeItem { get; set; }

        INewElementExtensionTreeNode SelectedCategory { get; set; }

    }

    public interface IExtensionDialogItemPresenter<out T> : IExtensionDialogItemPresenter
    {
        T CreateResult(string name, string path);
    }
}