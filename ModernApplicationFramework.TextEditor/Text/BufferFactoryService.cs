using System;
using System.Collections;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Text.Differencing;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Text
{
    [Export(typeof(ITextBufferFactoryService))]
    internal class BufferFactoryService : ITextBufferFactoryService
    {
        [Export]
        [Name("any")]
        public ContentTypeDefinition AnyContentTypeDefinition;
        [Export]
        [Name("text")]
        [BaseDefinition("any")]
        public ContentTypeDefinition TextContentTypeDefinition;
        [Export]
        [Name("projection")]
        [BaseDefinition("any")]
        public ContentTypeDefinition ProjectionContentTypeDefinition;
        [Export]
        [Name("plaintext")]
        [BaseDefinition("text")]
        public ContentTypeDefinition PlaintextContentTypeDefinition;

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import]
        internal GuardedOperations _guardedOperations { get; set; }

        [Import]
        internal ITextDifferencingSelectorService _textDifferencingSelectorService { get; set; }


        private IContentType _textContentType;
        private IContentType _plaintextContentType;
        private IContentType _inertContentType;

        public IContentType TextContentType => _textContentType ?? (_textContentType = ContentTypeRegistryService.GetContentType("text"));

        public IContentType PlaintextContentType => _plaintextContentType ??
                                                    (_plaintextContentType = ContentTypeRegistryService.GetContentType("plaintext"));

        public IContentType InertContentType => _inertContentType ?? (_inertContentType = ContentTypeRegistryService.GetContentType("inert"));

        public ITextBuffer CreateTextBuffer()
        {
            return Make(TextContentType, StringRebuilder.Empty, false);
        }

        public ITextBuffer CreateTextBuffer(IContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            return Make(contentType, StringRebuilder.Empty, false);
        }

        public ITextBuffer CreateTextBuffer(string text, IContentType contentType)
        {
            return CreateTextBuffer(text, contentType, false);
        }

        public ITextBuffer CreateTextBuffer(SnapshotSpan span, IContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            var content = StringRebuilderFromSnapshotSpan(span);
            return Make(contentType, content, false);
        }

       
        public ITextBuffer CreateTextBuffer(string text, IContentType contentType, bool spurnGroup)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            return Make(contentType, StringRebuilder.Create(text), spurnGroup);
        }

        internal static StringRebuilder StringRebuilderFromSnapshotSpan(SnapshotSpan span)
        {
            return StringRebuilderFromSnapshotAndSpan(span.Snapshot, span.Span);
        }

        internal static StringRebuilder StringRebuilderFromSnapshotAndSpan(ITextSnapshot snapshot, Span span)
        {
            return AppendStringRebuildersFromSnapshotAndSpan(StringRebuilder.Empty, snapshot, span);
        }

        internal static StringRebuilder AppendStringRebuildersFromSnapshotAndSpan(StringRebuilder content, ITextSnapshot snapshot, Span span)
        {
            content = !(snapshot is BaseSnapshot baseSnapshot)
                ? content.Append(snapshot.GetText(span))
                : content.Append(baseSnapshot.Content.GetSubText(span));
            return content;
        }

        private TextBuffer Make(IContentType contentType, StringRebuilder content, bool spurnGroup)
        {
            var textBuffer = new TextBuffer(contentType, content, _textDifferencingSelectorService.DefaultTextDifferencingService, _guardedOperations, spurnGroup);
            RaiseTextBufferCreatedEvent(textBuffer);
            return textBuffer;
        }

        private void RaiseTextBufferCreatedEvent(ITextBuffer buffer)
        {
            var textBufferCreated = TextBufferCreated;
            textBufferCreated?.Invoke(this, new TextBufferCreatedEventArgs(buffer));
        }

        public event EventHandler<TextBufferCreatedEventArgs> TextBufferCreated;
    }
}
