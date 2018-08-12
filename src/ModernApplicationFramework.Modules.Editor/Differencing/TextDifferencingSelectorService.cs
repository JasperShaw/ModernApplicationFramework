using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    [Export(typeof(ITextDifferencingSelectorService))]
    internal class TextDifferencingSelectorService : ITextDifferencingSelectorService
    {
        private static readonly DefaultTextDifferencingService _defaultTextDifferencingService =
            new DefaultTextDifferencingService();

        public ITextDifferencingService DefaultTextDifferencingService => _defaultTextDifferencingService;

        [Import] internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        [ImportMany(typeof(ITextDifferencingService))]
        internal List<Lazy<ITextDifferencingService, IContentTypeMetadata>> TextDifferencingServices { get; set; }

        public ITextDifferencingService GetTextDifferencingService(IContentType contentType)
        {
            var guardedOperations = GuardedOperations;
            var differencingServices = TextDifferencingServices;
            var dataContentType = contentType;
            var typeRegistryService = ContentTypeRegistryService;
            return guardedOperations
                       .InvokeBestMatchingFactory(differencingServices, dataContentType, differencingService =>
                           differencingService, typeRegistryService, this) ?? DefaultTextDifferencingService;
        }
    }
}