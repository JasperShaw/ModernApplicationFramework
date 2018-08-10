using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Text;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ISmartIndentationService))]
    internal sealed class SmartIndentationService : ISmartIndentationService, ISmartIndent
    {
        [ImportMany]
        internal List<Lazy<ISmartIndentProvider, IContentTypeMetadata>> SmartIndentProviders { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        public int? GetDesiredIndentation(ITextView textView, ITextSnapshotLine line)
        {
            return GetSmartIndent(textView).GetDesiredIndentation(line);
        }

        private ISmartIndent GetSmartIndent(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(typeof(SmartIndentationService), () =>
            {
                EventHandler<TextDataModelContentTypeChangedEventArgs> onContentTypeChanged = null;
                EventHandler onClosed = null;

                void Disconnect()
                {
                    var property = (ISmartIndent) textView.Properties[typeof(SmartIndentationService)];
                    textView.Properties.RemoveProperty(typeof(SmartIndentationService));
                    property.Dispose();
                    textView.TextDataModel.ContentTypeChanged -= onContentTypeChanged;
                    textView.Closed -= onClosed;
                }

                onContentTypeChanged = (sender, e) => Disconnect();
                onClosed = (sender, e) => Disconnect();
                textView.TextDataModel.ContentTypeChanged += onContentTypeChanged;
                textView.Closed += onClosed;
                return CreateSmartIndent(textView);
            });
        }

        private ISmartIndent CreateSmartIndent(ITextView textView)
        {
            if (textView.IsClosed)
                return this;
            return GuardedOperations.InvokeBestMatchingFactory(SmartIndentProviders, textView.TextDataModel.ContentType, provider => provider.CreateSmartIndent(textView), ContentTypeRegistryService, this) ?? this;
        }

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            return new int?();
        }

        public void Dispose()
        {
        }
    }
}