using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.OverviewMargin;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export]
    internal sealed class OverviewElementFactory
    {
        [ImportMany]
        private List<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _tipProviders;
        private IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> _orderedTipProviders;

        [Import]
        internal IScrollMapFactoryService ScrollMapFactory { get; private set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; private set; }

        [Import]
        internal IGuardedOperations GuardedOperations { get; private set; }

        [Import(AllowDefault = true)]
        internal IOutliningManagerService OutliningManagerService { get; private set; }

        [Import]
        internal ITextEditorFactoryService EditorFactory { get; private set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; private set; }

        internal IList<Lazy<IOverviewTipManagerProvider, ITipMetadata>> OrderedTipProviders => _orderedTipProviders ?? (_orderedTipProviders = Orderer.Order(_tipProviders));

        public OverviewElement CreateElement(ITextViewHost textViewHost)
        {
            return OverviewElement.Create(textViewHost, this);
        }
    }
}
