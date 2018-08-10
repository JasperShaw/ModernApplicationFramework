using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    internal static class ExtensionSelector
    {
        public static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, IContentType dataContentType) where TMetadataView : IContentTypeMetadata
        {
            return providerHandles.Where(providerHandle =>
                ContentTypeMatch(dataContentType, providerHandle.Metadata.ContentTypes)).ToList();
        }

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
