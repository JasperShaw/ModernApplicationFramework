using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface ICommandingTextBufferResolver
    {
        IEnumerable<ITextBuffer> ResolveBuffersForCommand<TArgs>() where TArgs : EditorCommandArgs;
    }
}