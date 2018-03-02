using System.Collections.Generic;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFileDefinitionContextManager
    {
        IEnumerable<IFileDefinitionContext> GetRegisteredFileDefinitionContexts { get; }
    }
}