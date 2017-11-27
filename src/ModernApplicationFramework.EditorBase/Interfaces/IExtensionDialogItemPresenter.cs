using System.Collections.Generic;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDialogItemPresenter : IListViewContainer
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }

        IEnumerable<IExtensionDefinition> ItemSource { get; set; }

        object CreateResult(string name, string path);

        IExtensionDefinition SelectedItem { get; }
    }
}