using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Commanding
{
    public interface ICommandingTextBufferResolver
    {
        IEnumerable<ITextBuffer> ResolveBuffersForCommand<TArgs>() where TArgs : EditorCommandArgs;
    }
}