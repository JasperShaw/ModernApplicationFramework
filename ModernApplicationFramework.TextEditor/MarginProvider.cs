using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    internal abstract class MarginProvider : ITextViewMarginProvider
    {
        private List<string> _orderedErrorTypes;

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; private set; }

        [Import]
        internal IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService { get; private set; }

        [ImportMany]
        internal IEnumerable<Lazy<ErrorTypeDefinition, IOrderable>> ErrorTypes { get; private set; }

        protected abstract BaseMarginElement CreateMarginElement(ITextView textView, IVerticalScrollBar scrollbar, MarginProvider provider, List<string> orderedErrorTypes);

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin parentMargin)
        {
            var scrollbar = parentMargin as IVerticalScrollBar;
            if (scrollbar == null)
                return null;
            if (_orderedErrorTypes == null)
            {
                var lazyList = Orderer.Order(ErrorTypes);
                var stringList = new List<string>(lazyList.Count);
                stringList.AddRange(lazyList.Select(lazy => lazy.Metadata.Name));
                _orderedErrorTypes = stringList;
            }

            return OverviewMarkMargin.Create(CreateMarginElement(textViewHost.TextView,
                scrollbar, this, _orderedErrorTypes));
        }
    }
}