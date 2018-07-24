using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    [Export(typeof(ITextDifferencingSelectorService))]
    internal class TextDifferencingSelectorService : ITextDifferencingSelectorService
    {
        private static readonly DefaultTextDifferencingService _defaultTextDifferencingService = new DefaultTextDifferencingService();

        [ImportMany(typeof(ITextDifferencingService))]
        internal List<Lazy<ITextDifferencingService, IContentTypeMetadata>> _textDifferencingServices { get; set; }

        [Import]
        internal IContentTypeRegistryService _contentTypeRegistryService { get; set; }

        [Import]
        internal GuardedOperations _guardedOperations { get; set; }

        public ITextDifferencingService GetTextDifferencingService(IContentType contentType)
        {
            var guardedOperations = _guardedOperations;
            var differencingServices = _textDifferencingServices;
            var dataContentType = contentType;
            var typeRegistryService = _contentTypeRegistryService;
            return guardedOperations
                       .InvokeBestMatchingFactory(differencingServices, dataContentType, differencingService =>
                               differencingService, typeRegistryService, this) ??  DefaultTextDifferencingService;
        }

        public ITextDifferencingService DefaultTextDifferencingService => _defaultTextDifferencingService;
    }
}
