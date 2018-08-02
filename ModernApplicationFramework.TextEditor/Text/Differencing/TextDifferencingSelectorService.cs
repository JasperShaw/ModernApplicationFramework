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
        internal List<Lazy<ITextDifferencingService, IContentTypeMetadata>> TextDifferencingServices { get; set; }

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        public ITextDifferencingService GetTextDifferencingService(IContentType contentType)
        {
            var guardedOperations = GuardedOperations;
            var differencingServices = TextDifferencingServices;
            var dataContentType = contentType;
            var typeRegistryService = ContentTypeRegistryService;
            return guardedOperations
                       .InvokeBestMatchingFactory(differencingServices, dataContentType, differencingService =>
                               differencingService, typeRegistryService, this) ??  DefaultTextDifferencingService;
        }

        public ITextDifferencingService DefaultTextDifferencingService => _defaultTextDifferencingService;
    }
}
