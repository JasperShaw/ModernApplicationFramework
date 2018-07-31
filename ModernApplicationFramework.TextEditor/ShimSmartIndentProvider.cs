﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ISmartIndentProvider))]
    [ContentType("text")]
    internal sealed class ShimSmartIndentProvider : ISmartIndentProvider
    {
        public ISmartIndent CreateSmartIndent(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return new ShimSmartIndent(textView);
        }
    }
}