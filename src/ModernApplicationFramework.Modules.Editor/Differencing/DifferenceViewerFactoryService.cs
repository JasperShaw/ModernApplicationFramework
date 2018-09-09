using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    [Export(typeof(IDifferenceViewerFactoryService))]
    internal class DifferenceViewerFactoryService : IDifferenceViewerFactoryService
    {
        [Import]
        internal ITextEditorFactoryService EditorFactory;
        [Import]
        internal IEditorOptionsFactoryService OptionsFactory;
        [Import]
        internal IEditorOperationsFactoryService OperationsFactory;

        //TODO: Undo
        //[Import]
        //internal ITextUndoHistoryRegistry UndoHistoryRegistry;
        //[Import]
        //internal ITextBufferUndoManagerProvider TextBufferUndoManagerProvider;
        [Import]
        internal IEditorFormatMapService FormatMapService;

        public IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, IEditorOptions parentOptions = null)
        {
            return CreateDifferenceView(buffer, EditorFactory.DefaultRoles, parentOptions);
        }

        public IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, ITextViewRoleSet roles, IEditorOptions parentOptions = null)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            void CreateTextViewCallback(IDifferenceTextViewModel textViewModel, ITextViewRoleSet baseRoles, IEditorOptions options, out FrameworkElement visualElement, out ITextViewHost textViewHost)
            {
                ITextViewRoleSet textViewRoleSet = EditorFactory.CreateTextViewRoleSet(roles.Concat(baseRoles));
                ITextView textView = EditorFactory.CreateTextView(textViewModel, textViewRoleSet, options);
                textViewHost = EditorFactory.CreateTextViewHost(textView, false);
                visualElement = textViewHost.HostControl;
            }

            return CreateDifferenceView(buffer, CreateTextViewCallback, parentOptions);
        }

        public IDifferenceViewer CreateDifferenceView(IDifferenceBuffer buffer, CreateTextViewHostCallback createTextViewCallback, IEditorOptions parentOptions = null)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (createTextViewCallback == null)
                throw new ArgumentNullException(nameof(createTextViewCallback));
            DifferenceViewer differenceViewer = new DifferenceViewer(this);
            differenceViewer.Initialize(buffer, createTextViewCallback, parentOptions);
            return differenceViewer;
        }

        public IDifferenceViewer CreateUninitializedDifferenceView()
        {
            return new DifferenceViewer(this);
        }

        public IDifferenceViewer TryGetViewerForTextView(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return (textView.TextViewModel as IDifferenceTextViewModel)?.Viewer;
        }
    }
}