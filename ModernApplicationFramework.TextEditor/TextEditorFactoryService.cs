using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextEditorFactoryService))]
    internal sealed class TextEditorFactoryService : ITextEditorFactoryService, IPartImportsSatisfiedNotification
    {
        internal Dictionary<string, int> OrderedViewLayerDefinitions = new Dictionary<string, int>();
        internal Dictionary<string, int> OrderedOverlayLayerDefinitions = new Dictionary<string, int>();
        internal Dictionary<string, int> OrderedSpaceReservationManagerDefinitions = new Dictionary<string, int>();

        public ITextViewRoleSet DefaultRoles => CreateTextViewRoleSet("ANALYZABLE", "DOCUMENT", "EDITABLE",
            "INTERACTIVE", "STRUCTURED", "ZOOMABLE");

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        [ImportMany]
        private List<Lazy<AdornmentLayerDefinition, IAdornmentLayersMetadata>> _viewLayerDefinitions;

        [ImportMany]
        private List<Lazy<SpaceReservationManagerDefinition, IOrderable>> _spaceReservationManagerDefinitions;

        [Import]
        internal TextViewMarginState MarginState { get; set; }

        [Import]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        [ImportMany]
        internal List<Lazy<ILineTransformSourceProvider, IContentTypeAndTextViewRoleMetadata>> LineTransformSourceProviders { get; set; }

        [ImportMany(typeof(ITextViewCreationListener))]
        internal List<Lazy<ITextViewCreationListener, IDeferrableContentTypeAndTextViewRoleMetadata>> TextViewCreationListeners { get; set; }

        [ImportMany(typeof(ITextViewConnectionListener))]
        internal List<Lazy<ITextViewConnectionListener, IContentTypeAndTextViewRoleMetadata>> TextViewConnectionListeners { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        [Import(AllowDefault = true)]
        internal IOutliningManagerService OutliningManagerService { get; set; }

        [Import]
        internal ISmartIndentationService SmartIndentationService { get; set; }

        [Import]
        internal IBufferGraphFactoryService BufferGraphFactoryService { get; set; }

        [Import]
        internal IViewClassifierAggregatorService ClassifierAggregatorService { get; set; }

        [Import]
        internal IFormattedTextSourceFactoryService FormattedTextSourceFactoryService { get; set; }

        [Import]
        internal ITextAndAdornmentSequencerFactoryService TextAndAdornmentSequencerFactoryService { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMappingService { get; set; }

        [Import]
        internal PerformanceBlockMarker PerformanceBlockMarker { get; set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }


        public event EventHandler<TextViewCreatedEventArgs> TextViewCreated;

        private TextView CreateAndTrackTextView(/*ITextViewModel viewModel, */bool initialize = true)
        {

            var buffer = TextBufferFactoryService.CreateTextBuffer();
            var dataModel = new VacuousTextDataModel(buffer);
            ITextViewModel viewModel = new VacuousTextViewModel(dataModel);

            viewModel.DataBuffer.Insert(0, "Hallo Welt\r\nTest");

            var textView = new TextView(viewModel, DefaultRoles, EditorOptionsFactoryService.GlobalOptions, this, false);
            if (initialize)
                InitializeTextView(textView);
            return textView;
        }

        private void InitializeTextView(ITextView view)
        {
            if (!(view is TextView textView) || textView.IsTextViewInitialized)
                throw new ArgumentException();
            textView.Initialize();

            //TODO: Guarded operations

            var textViewCreated = TextViewCreated;
            textViewCreated?.Invoke(this, new TextViewCreatedEventArgs(textView));
        }

        public ITextView CreateTextView()
        {
            return CreateAndTrackTextView();
        }

        public ITextViewHost CreateTextViewHost(ITextView textView, bool setFocus)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return new TextViewHost(textView, setFocus, this);
        }

        public ITextViewHost CreateTextViewHostWithoutInitialization(ITextView textView, bool setFocus)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return new TextViewHost(textView, setFocus, this, false);
        }


        public void OnImportsSatisfied()
        {
            var lazyList = Orderer.Order(_viewLayerDefinitions);
            for (var index = 0; index < lazyList.Count; ++index)
            {
                if (lazyList[index].Metadata.IsOverlayLayer)
                    OrderedOverlayLayerDefinitions.Add(lazyList[index].Metadata.Name, index);
                else
                    OrderedViewLayerDefinitions.Add(lazyList[index].Metadata.Name, index);
            }

            var lazyList2 = Orderer.Order(_spaceReservationManagerDefinitions);
            for (var index = 0; index < lazyList2.Count; ++index)
                OrderedSpaceReservationManagerDefinitions.Add(lazyList2[index].Metadata.Name, index);
        }

        public ITextViewRoleSet CreateTextViewRoleSet(params string[] roles)
        {
            return new TextViewRoleSet(roles);
        }
    }
}
