using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    [Export(typeof(ITextStructureNavigatorSelectorService))]
    internal sealed class TextStructureNavigatorSelectorService : ITextStructureNavigatorSelectorService
    {
        [Import] internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        [ImportMany(typeof(ITextStructureNavigatorProvider))]
        internal List<Lazy<ITextStructureNavigatorProvider, IContentTypeMetadata>> TextStructureNavigatorProviders
        {
            get;
            set;
        }

        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer, IContentType contentType)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            return CreateNavigator(textBuffer, contentType);
        }

        public ITextStructureNavigator GetTextStructureNavigator(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (textBuffer.Properties.TryGetProperty(typeof(ITextStructureNavigator),
                out ITextStructureNavigator property))
                return property;
            property = CreateNavigator(textBuffer, textBuffer.ContentType);
            textBuffer.Properties[typeof(ITextStructureNavigator)] = property;
            textBuffer.ContentTypeChanged += OnContentTypeChanged;
            return property;
        }

        private ITextStructureNavigator CreateNavigator(ITextBuffer textBuffer, IContentType contentType)
        {
            return GuardedOperations
                       .InvokeBestMatchingFactory(
                           TextStructureNavigatorProviders, contentType,
                           provider =>
                               provider.CreateTextStructureNavigator(textBuffer), ContentTypeRegistryService,
                           this) ??
                   new DefaultTextNavigator(textBuffer, ContentTypeRegistryService);
        }

        private void OnContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            var textBuffer = e.Before.TextBuffer;
            textBuffer.Properties.RemoveProperty(typeof(ITextStructureNavigator));
            textBuffer.ContentTypeChanged -= OnContentTypeChanged;
        }
    }
}