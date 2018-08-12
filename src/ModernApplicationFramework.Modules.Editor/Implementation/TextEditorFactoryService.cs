using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(ITextEditorFactoryService))]
    internal sealed class TextEditorFactoryService : ITextEditorFactoryService, IPartImportsSatisfiedNotification
    {
        internal Dictionary<string, int> OrderedViewLayerDefinitions = new Dictionary<string, int>();
        internal Dictionary<string, int> OrderedOverlayLayerDefinitions = new Dictionary<string, int>();
        internal Dictionary<string, int> OrderedSpaceReservationManagerDefinitions = new Dictionary<string, int>();

        public ITextViewRoleSet NoRoles => new TextViewRoleSet(new string[0]);
        public ITextViewRoleSet AllPredefinedRoles => CreateTextViewRoleSet("ANALYZABLE", "DEBUGGABLE", "DOCUMENT", "EDITABLE", "INTERACTIVE", "STRUCTURED", "ZOOMABLE", "PRIMARYDOCUMENT");

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

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [ImportMany]
        internal List<Lazy<ITextViewModelProvider, IContentTypeAndTextViewRoleMetadata>> TextViewModelProviders { get; set; }

        [ImportMany]
        internal List<Lazy<IObjectTracker>> ObjectTrackers { get; set; }


        public void InitializeTextViewHost(ITextViewHost host)
        {
            if (!(host is TextViewHost textViewHost) || textViewHost.IsTextViewHostInitialized)
                throw new ArgumentException();
            textViewHost.Initialize();
        }

        public event EventHandler<TextViewCreatedEventArgs> TextViewCreated;

        public ITextView CreateTextViewWithoutInitialization(ITextDataModel dataModel, ITextViewRoleSet roles,
            IEditorOptions parentOptions)
        {
            if (dataModel == null)
                throw new ArgumentNullException(nameof(dataModel));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            if (parentOptions == null)
                throw new ArgumentNullException(nameof(parentOptions));
            return CreateAndTrackTextView(dataModel, roles, parentOptions, false);
        }

        private TextView CreateAndTrackTextView(ITextDataModel dataModel, ITextViewRoleSet roles, IEditorOptions parentOptions, bool initialize = true)
        {
            return CreateAndTrackTextView(
                UiExtensionSelector
                    .InvokeBestMatchingFactory(
                        TextViewModelProviders, dataModel.ContentType, roles,
                        provider => provider.CreateTextViewModel(dataModel, roles), ContentTypeRegistryService,
                        GuardedOperations, this) ?? new VacuousTextViewModel(dataModel),
                roles, parentOptions, initialize);
        }

        private TextView CreateAndTrackTextView(ITextViewModel viewModel, ITextViewRoleSet roles,
            IEditorOptions parentOptions, bool initialize = true)
        {
            var wpfTextView = new TextView(viewModel, roles, parentOptions, this, false);
            if (initialize)
                InitializeTextView(wpfTextView);
            return wpfTextView;
        }

        void ITextEditorFactoryService.InitializeTextView(ITextView view)
        {
            InitializeTextView(view);
        }

        private void InitializeTextView(ITextView view)
        {
            if (!(view is TextView textView) || textView.IsTextViewInitialized)
                throw new ArgumentException();
            textView.Initialize();

            foreach (Lazy<IObjectTracker> objectTracker in ObjectTrackers)
                GuardedOperations.InstantiateExtension(objectTracker, objectTracker)?.TrackObject(view, "Text Views");

            var textViewCreated = TextViewCreated;
            textViewCreated?.Invoke(this, new TextViewCreatedEventArgs(textView));
        }

        public ITextView CreateTextView()
        {
            return CreateTextView(TextBufferFactoryService.CreateTextBuffer(), DefaultRoles);
        }

        public ITextView CreateTextView(ITextBuffer textBuffer, ITextViewRoleSet roles)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            return CreateTextView(new VacuousTextDataModel(textBuffer), roles, EditorOptionsFactoryService.GlobalOptions);
        }

        public ITextView CreateTextView(ITextDataModel dataModel, ITextViewRoleSet roles, IEditorOptions parentOptions)
        {
            if (dataModel == null)
                throw new ArgumentNullException(nameof(dataModel));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            if (parentOptions == null)
                throw new ArgumentNullException(nameof(parentOptions));
            return CreateAndTrackTextView(dataModel, roles, parentOptions);
        }

        public ITextView CreateTextView(ITextViewModel viewModel, ITextViewRoleSet roles, IEditorOptions parentOptions)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            if (parentOptions == null)
                throw new ArgumentNullException(nameof(parentOptions));
            return CreateAndTrackTextView(viewModel, roles, parentOptions);
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

        public ITextViewRoleSet CreateTextViewRoleSet(IEnumerable<string> roles)
        {
            return new TextViewRoleSet(roles);
        }

        public ITextViewRoleSet CreateTextViewRoleSet(params string[] roles)
        {
            return new TextViewRoleSet(roles);
        }
    }
}
