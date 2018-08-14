﻿namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IViewPrimitivesFactoryService
    {
        PrimitiveTextView CreateTextView(ITextView textView);

        DisplayTextPoint CreateDisplayTextPoint(PrimitiveTextView textView, int position);

        DisplayTextRange CreateDisplayTextRange(PrimitiveTextView textView, TextRange textRange);

        Selection CreateSelection(PrimitiveTextView textView);

        Caret CreateCaret(PrimitiveTextView textView);
    }
}