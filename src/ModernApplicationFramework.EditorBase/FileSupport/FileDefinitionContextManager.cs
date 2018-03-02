using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Export(typeof(IFileDefinitionContextManager))]
    public class FileDefinitionContextManager : IFileDefinitionContextManager
    {
        public IEnumerable<IFileDefinitionContext> GetRegisteredFileDefinitionContexts { get; }

        [ImportingConstructor]
        public FileDefinitionContextManager([ImportMany]IFileDefinitionContext[] definitionContexts)
        {
            GetRegisteredFileDefinitionContexts = definitionContexts;
        }
    }
}