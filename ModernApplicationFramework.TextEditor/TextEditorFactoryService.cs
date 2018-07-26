using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextEditorFactoryService))]
    internal sealed class TextEditorFactoryService : ITextEditorFactoryService, IPartImportsSatisfiedNotification
    {
        internal Dictionary<string, int> OrderedViewLayerDefinitions = new Dictionary<string, int>();
        internal Dictionary<string, int> OrderedOverlayLayerDefinitions = new Dictionary<string, int>();

        public ITextViewRoleSet DefaultRoles => CreateTextViewRoleSet("ANALYZABLE", "DOCUMENT", "EDITABLE",
            "INTERACTIVE", "STRUCTURED", "ZOOMABLE");

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        [ImportMany]
        private List<Lazy<AdornmentLayerDefinition, IAdornmentLayersMetadata>> _viewLayerDefinitions;

        [Import]
        internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

        [Import]
        internal IBufferGraphFactoryService BufferGraphFactoryService { get; set; }

        public event EventHandler<TextViewCreatedEventArgs> TextViewCreated;

        private TextView CreateAndTrackTextView(/*ITextViewModel viewModel, */bool initialize = true)
        {

            var buffer = TextBufferFactoryService.CreateTextBuffer();
            var dataModel = new VacuousTextDataModel(buffer);
            ITextViewModel viewModel = new VacuousTextViewModel(dataModel);

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

            EventHandler<TextViewCreatedEventArgs> textViewCreated = TextViewCreated;
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
        }

        public ITextViewRoleSet CreateTextViewRoleSet(params string[] roles)
        {
            return new TextViewRoleSet(roles);
        }
    }
}
