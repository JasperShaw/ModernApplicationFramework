﻿using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IEditorPrimitivesFactoryService))]
    internal sealed class EditorPrimitivesFactoryService : IEditorPrimitivesFactoryService
    {
        [Import]
        internal IViewPrimitivesFactoryService ViewPrimitivesFactory { get; set; }

        [Import]
        internal IBufferPrimitivesFactoryService BufferPrimitivesFactory { get; set; }

        public IViewPrimitives GetViewPrimitives(ITextView textView)
        {
            return new ViewPrimitives(textView, ViewPrimitivesFactory);
        }

        public IBufferPrimitives GetBufferPrimitives(ITextBuffer textBuffer)
        {
            return new BufferPrimitives(textBuffer, BufferPrimitivesFactory);
        }
    }
}