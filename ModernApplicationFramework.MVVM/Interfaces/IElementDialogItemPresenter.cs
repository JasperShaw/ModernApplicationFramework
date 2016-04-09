using System.Collections.Generic;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IElementDialogItemPresenter
    {
        bool UsesNameProperty { get; }

        bool UsesPathProperty { get; }
        IEnumerable<object> ItemSource { get; set; }

        object CreateResult(string name, string path);
    }
}