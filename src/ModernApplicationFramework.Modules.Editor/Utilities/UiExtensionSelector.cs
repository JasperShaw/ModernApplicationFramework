using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal static class UiExtensionSelector
    {
        public static TExtensionInstance
            InvokeBestMatchingFactory<TExtensionInstance, TExtensionFactory, TMetadataView>(
                IEnumerable<Lazy<TExtensionFactory, TMetadataView>> providerHandles, IContentType dataContentType,
                ITextViewRoleSet viewRoles, Func<TExtensionFactory, TExtensionInstance> getter,
                IContentTypeRegistryService contentTypeRegistryService, GuardedOperations guardedOperations,
                object errorSource) where TExtensionInstance : class
            where TExtensionFactory : class
            where TMetadataView : IContentTypeAndTextViewRoleMetadata
        {
            var lazyList = SelectMatchingExtensions(providerHandles, viewRoles);
            return guardedOperations.InvokeBestMatchingFactory(lazyList, dataContentType, getter,
                contentTypeRegistryService, errorSource);
        }

        public static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(
            IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, ITextViewRoleSet viewRoles)
            where TMetadataView : ITextViewRoleMetadata
        {
            return (from providerHandle in providerHandles
                let textViewRoles = providerHandle.Metadata.TextViewRoles
                where viewRoles.ContainsAny(textViewRoles)
                select providerHandle).ToList();
        }

        public static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(
            IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, IContentType documentContentType,
            IContentType excludedContentType, ITextViewRoleSet viewRoles)
            where TMetadataView : IContentTypeAndTextViewRoleMetadata
        {
            var lazyList = new List<Lazy<TProvider, TMetadataView>>();
            foreach (var providerHandle in providerHandles)
            {
                TMetadataView metadata;
                if (excludedContentType != null)
                {
                    var dataContentType = excludedContentType;
                    metadata = providerHandle.Metadata;
                    var contentTypes = metadata.ContentTypes;
                    if (ExtensionSelector.ContentTypeMatch(dataContentType, contentTypes))
                        continue;
                }

                var dataContentType1 = documentContentType;
                metadata = providerHandle.Metadata;
                var contentTypes1 = metadata.ContentTypes;
                if (ExtensionSelector.ContentTypeMatch(dataContentType1, contentTypes1))
                {
                    var textViewRoleSet = viewRoles;
                    metadata = providerHandle.Metadata;
                    var textViewRoles = metadata.TextViewRoles;
                    if (textViewRoleSet.ContainsAny(textViewRoles))
                        lazyList.Add(providerHandle);
                }
            }

            return lazyList;
        }
    }
}