using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport.TextFile
{
    [Export(typeof(TextFileDefinitionContext))]
    [Export(typeof(IFileDefinitionContext))]
    public class TextFileDefinitionContext : IFileDefinitionContext
    {
        public string Context => FileSupportResources.TextFileDefinitionContext;
    }
}