using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.Interfaces.Editor
{
    public interface INormalEditor : IEditor<IDocument>
    {
        IDocument Document { get; }
    }
}