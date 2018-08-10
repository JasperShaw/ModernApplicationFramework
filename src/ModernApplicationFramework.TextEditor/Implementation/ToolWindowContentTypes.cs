using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal static class ToolWindowContentTypes
    {
        [Export]
        [Name("Output")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition OutputContentType;
    }
}
