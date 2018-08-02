using Caliburn.Micro;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal static class EditorParts
    {
        public static IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService
        {
            get => EditorPartsRepository.ViewTagAggregatorFactoryService ??
                   (EditorPartsRepository.ViewTagAggregatorFactoryService =
                       IoC.Get<IViewTagAggregatorFactoryService>());
            internal set => EditorPartsRepository.ViewTagAggregatorFactoryService = value;
        }

        public static ITextDocumentFactoryService TextDocumentFactoryService
        {
            get => EditorPartsRepository.TextDocumentFactoryService ?? (EditorPartsRepository.TextDocumentFactoryService =
                       IoC.Get<ITextDocumentFactoryService>());
            internal set => EditorPartsRepository.TextDocumentFactoryService = value;
        }

        public static ITextEditorFactoryService TextEditorFactoryService
        {
            get => EditorPartsRepository.TextEditorFactoryService ?? (EditorPartsRepository.TextEditorFactoryService =
                       IoC.Get<ITextEditorFactoryService>());
            internal set => EditorPartsRepository.TextEditorFactoryService = value;
        }

        public static IEditorFormatMapService EditorFormatMapService
        {
            get => EditorPartsRepository.EditorFormatMapService ?? (EditorPartsRepository.EditorFormatMapService =
                       IoC.Get<IEditorFormatMapService>());
            internal set => EditorPartsRepository.EditorFormatMapService = value;
        }

        public static IClassificationFormatMapService ClassificationFormatMapService
        {
            get => EditorPartsRepository.ClassificationFormatMapService ?? (EditorPartsRepository.ClassificationFormatMapService =
                       IoC.Get<IClassificationFormatMapService>());
            internal set => EditorPartsRepository.ClassificationFormatMapService = value;
        }

        public static IEditorOptionsFactoryService EditorOptionsFactoryService
        {
            get => EditorPartsRepository.EditorOptionsFactoryService ?? (EditorPartsRepository.EditorOptionsFactoryService =
                       IoC.Get<IEditorOptionsFactoryService>());
            internal set => EditorPartsRepository.EditorOptionsFactoryService = value;
        }

        public static IGuardedOperations GuardedOperations
        {
            get
            {
                if (EditorPartsRepository.GuardedOperations != null)
                    return EditorPartsRepository.GuardedOperations;
                EditorPartsRepository.GuardedOperations = IoC.Get<IGuardedOperations>();
                Validate.IsNotNull(EditorPartsRepository.GuardedOperations, nameof(EditorPartsRepository.GuardedOperations));
                return EditorPartsRepository.GuardedOperations;
            }
            internal set => EditorPartsRepository.GuardedOperations = value;
        }

        public static ITextBufferFactoryService TextBufferFactoryService
        {
            get => EditorPartsRepository.TextBufferFactoryService ?? (EditorPartsRepository.TextBufferFactoryService =
                       IoC.Get<ITextBufferFactoryService>());
            internal set => EditorPartsRepository.TextBufferFactoryService = value;
        }

        private static class EditorPartsRepository
        {
            //internal static IComponentModel componentModel;
            internal static IGuardedOperations GuardedOperations;
            internal static IEditorOptionsFactoryService EditorOptionsFactoryService;
            internal static ITextDocumentFactoryService TextDocumentFactoryService;
            internal static IContentTypeRegistryService ContentTypeRegistryService;
            //internal static IFileExtensionRegistryService2 fileExtensionRegistryService;
            internal static IClassificationTypeRegistryService ClassificationTypeRegistryService;
            internal static IClassificationFormatMapService ClassificationFormatMapService;
            internal static ITextBufferFactoryService TextBufferFactoryService;
            //internal static IIntellisenseSessionStackMapService intellisenseSessionStackMapService;
            internal static ITextEditorFactoryService TextEditorFactoryService;
            internal static IEditorFormatMapService EditorFormatMapService;
            internal static IEditorOperationsFactoryService EditorOperationsFactoryService;
            //internal static ITextUndoHistoryRegistry textUndoHistoryRegistry;
            //internal static ITextBufferUndoManagerProvider textBufferUndoManagerProvider;
            internal static IBufferTagAggregatorFactoryService BufferTagAggregatorFactoryService;
            internal static IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService;
            internal static IOutliningManagerService OutliningManagerService;
            //internal static ICodingConventionsManager codingConventionsManager;
            //internal static IVsColorThemeService colorThemeService;
            //internal static Lazy<IVsUserEngagement> userEngagementService;
            //internal static IVsRunningDocumentTable runningDocumentTable;
            //internal static IVsTextManager vsTextManager;
        }
    }
}