﻿using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("HorizontalScrollBarContainer")]
    [MarginContainer("BottomControl")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    [GridUnitType(GridUnitType.Star)]
    internal sealed class HorizontalScrollBarContainerMarginProvider : ITextViewMarginProvider
    {
        [Import]
        private TextViewMarginState _marginState;
        [Import]
        private GuardedOperations _guardedOperations;

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return HorizontalScrollBarContainerMargin.Create(textViewHost, _guardedOperations, _marginState);
        }
    }
}