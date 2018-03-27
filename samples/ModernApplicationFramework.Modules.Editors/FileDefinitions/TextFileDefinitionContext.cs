using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.Modules.Editors.FileDefinitions
{
    [Export(typeof(TextFileDefinitionContext))]
    [Export(typeof(IFileDefinitionContext))]
    public class TextFileDefinitionContext : IFileDefinitionContext
    {
        public string Context => FileSupportResources.TextFileDefinitionContext;
    }
}