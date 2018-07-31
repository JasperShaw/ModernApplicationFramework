using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal static class ExtensionSelector
    {
        public static bool ContentTypeMatch(IContentType dataContentType, IEnumerable<string> extensionContentTypes)
        {
            return extensionContentTypes.Any(dataContentType.IsOfType);
        }

        public static bool ContentTypeMatch(IEnumerable<IContentType> dataContentTypes, IEnumerable<string> extensionContentTypes)
        {
            return dataContentTypes.Any(dataContentType => ContentTypeMatch(dataContentType, extensionContentTypes));
        }
    }
}
