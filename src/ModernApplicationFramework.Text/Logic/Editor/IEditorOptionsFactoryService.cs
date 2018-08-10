using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public interface IEditorOptionsFactoryService
    {
        IEditorOptions GlobalOptions { get; }

        IEditorOptions CreateOptions();
        IEditorOptions GetOptions(IPropertyOwner scope);
    }
}