using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public interface IEditorOptionsFactoryService
    {
        IEditorOptions GetOptions(IPropertyOwner scope);

        IEditorOptions CreateOptions();

        IEditorOptions GlobalOptions { get; }
    }
}