﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("BottomControl")]
    [MarginContainer("Bottom")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class BottomControlMarginProvider : ITextViewMarginProvider
    {
        [Import] private TextViewMarginState _marginState;

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        public ITextViewMargin CreateMargin(ITextViewHost wpfTextViewHost, ITextViewMargin marginContainer)
        {
            return ContainerMargin.Create("BottomControl", Orientation.Vertical, wpfTextViewHost, GuardedOperations,
                _marginState);
        }
    }
}