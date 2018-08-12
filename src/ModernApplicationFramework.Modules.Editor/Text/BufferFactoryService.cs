using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using ModernApplicationFramework.Modules.Editor.Projection;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    [Export(typeof(ITextImageFactoryService))]
    [Export(typeof(ITextBufferFactoryService))]
    [Export(typeof(IProjectionBufferFactoryService))]
    internal class BufferFactoryService : ITextBufferFactoryService, IProjectionBufferFactoryService, IInternalTextBufferFactory,  ITextImageFactoryService
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
        [Export]
        [Name("code")]
        [BaseDefinition("text")]
        public ContentTypeDefinition CodeContentType;
        [Export]
        [Name("inert")]
        public ContentTypeDefinition InertContentTypeDefinition;


        private IContentType _textContentType;
        private IContentType _plaintextContentType;
        private IContentType _inertContentType;
        private IContentType _projectionContentType;

        [Import]
        internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import]
        internal IDifferenceService DifferenceService { get; set; }

        [Import]
        internal ITextDifferencingSelectorService TextDifferencingSelectorService { get; set; }

        [Import]
        internal GuardedOperations GuardedOperations { get; set; }

        public IContentType TextContentType => _textContentType ?? (_textContentType = ContentTypeRegistryService.GetContentType("text"));

        public IContentType PlaintextContentType => _plaintextContentType ??
                                                    (_plaintextContentType = ContentTypeRegistryService.GetContentType("plaintext"));

        public IContentType InertContentType => _inertContentType ?? (_inertContentType = ContentTypeRegistryService.GetContentType("inert"));

        public IContentType ProjectionContentType => _projectionContentType ??
                                                     (_projectionContentType = ContentTypeRegistryService.GetContentType("projection"));

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

        public ITextBuffer CreateTextBuffer(ITextImage image, IContentType contentType)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            var content = StringRebuilder.Create(image);
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

        public ITextBuffer CreateTextBuffer(TextReader reader, IContentType contentType, long length, string traceId)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (length > int.MaxValue)
                throw new InvalidOperationException();
            var content = TextImageLoader.Load(reader, length, traceId, out var hasConsistentLineEndings, out _);
            ITextBuffer textBuffer = Make(contentType, content, false);
            if (!hasConsistentLineEndings)
                textBuffer.Properties.AddProperty("InconsistentLineEndings", true);
            return textBuffer;
        }

        public ITextBuffer CreateTextBuffer(TextReader reader, IContentType contentType)
        {
            return CreateTextBuffer(reader, contentType, -1L, "legacy");
        }

        internal static StringRebuilder StringRebuilderFromSnapshotAndSpan(ITextSnapshot snapshot, Span span)
        {
            return AppendStringRebuildersFromSnapshotAndSpan(StringRebuilder.Empty, snapshot, span);
        }

        internal static StringRebuilder StringRebuilderFromSnapshotSpan(SnapshotSpan span)
        {
            return StringRebuilderFromSnapshotAndSpan(span.Snapshot, span.Span);
        }

        internal static StringRebuilder StringRebuilderFromSnapshotSpans(IList<SnapshotSpan> sourceSpans, Span selectedSourceSpans)
        {
            var content = StringRebuilder.Empty;
            for (var index = 0; index < selectedSourceSpans.Length; ++index)
            {
                var sourceSpan = sourceSpans[selectedSourceSpans.Start + index];
                content = AppendStringRebuildersFromSnapshotAndSpan(content, sourceSpan.Snapshot, sourceSpan.Span);
            }
            return content;
        }

        internal static StringRebuilder AppendStringRebuildersFromSnapshotAndSpan(StringRebuilder content, ITextSnapshot snapshot, Span span)
        {
            var baseSnapshot = snapshot as BaseSnapshot;
            content = baseSnapshot == null ? content.Append(snapshot.GetText(span)) : content.Append(baseSnapshot.Content.GetSubText(span));
            return content;
        }

        public ITextImage CreateTextImage(string text)
        {
            return CachingTextImage.Create(StringRebuilder.Create(text), null);
        }

        public ITextImage CreateTextImage(TextReader reader, long length)
        {
            return CachingTextImage.Create(TextImageLoader.Load(reader, length, string.Empty, out _, out _), null);
        }

        public ITextImage CreateTextImage(MemoryMappedFile source)
        {
            using (var viewStream = source.CreateViewStream())
            {
                using (var streamReader = new StreamReader(viewStream, Encoding.Unicode))
                    return CreateTextImage(streamReader, -1L);
            }
        }

        private TextBuffer Make(IContentType contentType, StringRebuilder content, bool spurnGroup)
        {
            var textBuffer = new TextBuffer(contentType, content, TextDifferencingSelectorService.DefaultTextDifferencingService, GuardedOperations, spurnGroup);
            RaiseTextBufferCreatedEvent(textBuffer);
            return textBuffer;
        }

        public IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver, IList<object> trackingSpans, ProjectionBufferOptions options, IContentType contentType)
        {
            if (trackingSpans == null)
                throw new ArgumentNullException(nameof(trackingSpans));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            IProjectionBuffer projectionBuffer = new ProjectionBuffer(this, projectionEditResolver, contentType, trackingSpans, DifferenceService, TextDifferencingSelectorService.DefaultTextDifferencingService, options, GuardedOperations);
            RaiseProjectionBufferCreatedEvent(projectionBuffer);
            return projectionBuffer;
        }

        public IProjectionBuffer CreateProjectionBuffer(IProjectionEditResolver projectionEditResolver, IList<object> trackingSpans, ProjectionBufferOptions options)
        {
            if (trackingSpans == null)
                throw new ArgumentNullException(nameof(trackingSpans));
            IProjectionBuffer projectionBuffer = new ProjectionBuffer(this, projectionEditResolver, ProjectionContentType, trackingSpans, DifferenceService, TextDifferencingSelectorService.DefaultTextDifferencingService, options, GuardedOperations);
            RaiseProjectionBufferCreatedEvent(projectionBuffer);
            return projectionBuffer;
        }

        public IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver, NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options, IContentType contentType)
        {
            if (exposedSpans == null)
                throw new ArgumentNullException(nameof(exposedSpans));
            if (exposedSpans.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(exposedSpans));
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (exposedSpans[0].Snapshot != exposedSpans[0].Snapshot.TextBuffer.CurrentSnapshot)
                throw new ArgumentException("Elision buffer must be created against the current snapshot of its source buffer");
            IElisionBuffer elisionBuffer = new ElisionBuffer(projectionEditResolver, contentType, exposedSpans[0].Snapshot.TextBuffer, exposedSpans, options, TextDifferencingSelectorService.DefaultTextDifferencingService, GuardedOperations);
            RaiseProjectionBufferCreatedEvent(elisionBuffer);
            return elisionBuffer;
        }

        public IElisionBuffer CreateElisionBuffer(IProjectionEditResolver projectionEditResolver, NormalizedSnapshotSpanCollection exposedSpans, ElisionBufferOptions options)
        {
            return CreateElisionBuffer(projectionEditResolver, exposedSpans, options, ProjectionContentType);
        }

        public event EventHandler<TextBufferCreatedEventArgs> TextBufferCreated;

        public event EventHandler<TextBufferCreatedEventArgs> ProjectionBufferCreated;

        private void RaiseTextBufferCreatedEvent(ITextBuffer buffer)
        {
            var textBufferCreated = TextBufferCreated;
            textBufferCreated?.Invoke(this, new TextBufferCreatedEventArgs(buffer));
        }

        private void RaiseProjectionBufferCreatedEvent(IProjectionBufferBase buffer)
        {
            var projectionBufferCreated = ProjectionBufferCreated;
            projectionBufferCreated?.Invoke(this, new TextBufferCreatedEventArgs(buffer));
        }
    }
}