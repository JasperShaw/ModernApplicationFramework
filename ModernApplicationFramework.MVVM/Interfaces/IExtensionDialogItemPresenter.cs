using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IExtensionDialogItemPresenter
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        object CreateResult(string name, string path);

        IExtensionDefinition SelectedItem { get; }

        event EventHandler<ItemsChangedEventArgs> OnSelectedItemChanged;

    }
}