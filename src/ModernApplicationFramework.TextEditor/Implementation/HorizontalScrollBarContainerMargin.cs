﻿using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class HorizontalScrollBarContainerMargin : ContainerMargin
    {
        public HorizontalScrollBarContainerMargin(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
            : base("HorizontalScrollBarContainer", Orientation.Horizontal, wpfTextViewHost, guardedOperations, marginState)
        {
            VerticalAlignment = VerticalAlignment.Bottom;
        }

        internal static HorizontalScrollBarContainerMargin Create(ITextViewHost wpfTextViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            var barContainerMargin = new HorizontalScrollBarContainerMargin(wpfTextViewHost, guardedOperations, marginState);
            barContainerMargin.Initialize();
            return barContainerMargin;
        }
    }
}