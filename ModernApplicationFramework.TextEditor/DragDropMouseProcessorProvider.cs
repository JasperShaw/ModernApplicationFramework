﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("DragDrop")]
    [Order(Before = "WordSelection")]
    [ContentType("Text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class DragDropMouseProcessorProvider : IMouseProcessorProvider
    {
        [ImportMany(typeof(IDropHandlerProvider))]
        internal List<Lazy<IDropHandlerProvider, IDropHandlerMetadata>> DropHandlerFactories { get; set; }

        [Import]
        internal IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        [Import]
        internal IRtfBuilderService RtfBuilderService { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMapService { get; set; }

        //[Import]
        //internal ITextUndoHistoryRegistry UndoHistoryRegistry { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        public IMouseProcessor GetAssociatedProcessor(ITextView wpfTextView)
        {
            if (wpfTextView == null)
                throw new ArgumentNullException(nameof(wpfTextView));
            return wpfTextView.Properties.GetOrCreateSingletonProperty(typeof(IDragDropMouseProcessor),
                () => new DragDropMouseProcessor(wpfTextView, DropHandlerFactories,
                    EditorOperationsFactoryService.GetEditorOperations(wpfTextView),
                    (IRtfBuilderService2) RtfBuilderService, ClassificationFormatMapService,
                    //UndoHistoryRegistry.RegisterHistory((object) wpfTextView.TextBuffer),
                    GuardedOperations));
        }
    }
}