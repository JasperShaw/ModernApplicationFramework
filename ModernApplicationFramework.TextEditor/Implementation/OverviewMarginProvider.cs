﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("VerticalScrollBar")]
    [MarginContainer("VerticalScrollBarContainer")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class OverviewMarginProvider : ITextViewMarginProvider
    {
        [ImportMany] private List<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _tipProviders;

        private IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _orderedTipProviders;

        [Import]
        internal TextViewMarginState MarginState { get; set; }

        [Import]
        internal IScrollMapFactoryService ScrollMapFactory { get; private set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; private set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; private set; }

        [Import(AllowDefault = true)]
        internal IOutliningManagerService OutliningManagerService { get; private set; }

        [Import]
        internal ITextEditorFactoryService EditorFactory { get; private set; }

        [Import]
        internal IProjectionBufferFactoryService ProjectionFactory { get; private set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; private set; }

        [Import(typeof(PerformanceBlockMarker))]
        internal PerformanceBlockMarker PerformanceBlockMarker { get; set; }

        internal IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> OrderedTipProviders => _orderedTipProviders ?? (_orderedTipProviders = Orderer.Order(_tipProviders));

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            if (textViewHost.TextView.Roles.Contains("PRIMARYDOCUMENT") || textViewHost.TextView.Roles.Contains("EMBEDDED_PEEK_TEXT_VIEW"))
                return OverviewMargin.Create(textViewHost, containerMargin, this);
            return new VerticalScrollBarMargin(textViewHost.TextView, ScrollMapFactory.Create(textViewHost.TextView), "VerticalScrollBar", PerformanceBlockMarker);
        }
    }
}