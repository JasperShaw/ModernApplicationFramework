using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Tagging
{
    [Export(typeof(IBufferTagAggregatorFactoryService))]
    [Export(typeof(IViewTagAggregatorFactoryService))]
    internal sealed class TagAggregatorFactoryService : IBufferTagAggregatorFactoryService,
        IViewTagAggregatorFactoryService
    {
        internal ImmutableDictionary<ContentAndTypeData, IEnumerable<Lazy<ITaggerProvider, INamedTaggerMetadata>>>
            BufferTaggerProviderMap =
                ImmutableDictionary<ContentAndTypeData, IEnumerable<Lazy<ITaggerProvider, INamedTaggerMetadata>>>.Empty;

        internal ImmutableDictionary<ContentAndTypeData, IEnumerable<Lazy<IViewTaggerProvider, IViewTaggerMetadata>>>
            ViewTaggerProviderMap =
                ImmutableDictionary<ContentAndTypeData, IEnumerable<Lazy<IViewTaggerProvider, IViewTaggerMetadata>>>
                    .Empty;

        [Import] internal IBufferGraphFactoryService BufferGraphFactoryService { get; set; }

        [ImportMany(typeof(ITaggerProvider))]
        internal List<Lazy<ITaggerProvider, INamedTaggerMetadata>> BufferTaggerProviders { get; set; }

        [Import]
        internal JoinableTaskContext JoinableTaskContext { get; set; }

        [Import] internal IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        [ImportMany(typeof(IViewTaggerProvider))]
        internal List<Lazy<IViewTaggerProvider, IViewTaggerMetadata>> ViewTaggerProviders { get; set; }

        public ITagAggregator<T> CreateTagAggregator<T>(ITextBuffer textBuffer) where T : ITag
        {
            return CreateTagAggregator<T>(textBuffer, TagAggregatorOptions.None);
        }

        public ITagAggregator<T> CreateTagAggregator<T>(ITextBuffer textBuffer, TagAggregatorOptions options)
            where T : ITag
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return new TagAggregator<T>(this, null, BufferGraphFactoryService.CreateBufferGraph(textBuffer), options);
        }

        public ITagAggregator<T> CreateTagAggregator<T>(ITextView textView) where T : ITag
        {
            return CreateTagAggregator<T>(textView, TagAggregatorOptions.None);
        }

        public ITagAggregator<T> CreateTagAggregator<T>(ITextView textView, TagAggregatorOptions options) where T : ITag
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return new TagAggregator<T>(this, textView, textView.BufferGraph, options);
        }

        internal IEnumerable<Lazy<ITaggerProvider, INamedTaggerMetadata>> GetBufferTaggersForType(IContentType type,
            Type taggerType)
        {
            var key = new ContentAndTypeData(type, taggerType);
            if (!BufferTaggerProviderMap.TryGetValue(key, out var taggers))
            {
                taggers = new List<Lazy<ITaggerProvider, INamedTaggerMetadata>>(
                    BufferTaggerProviders.Where(f => Match(type, taggerType, f.Metadata)));
                ImmutableInterlocked.Update(ref BufferTaggerProviderMap, s => s.Add(key, taggers));
            }

            return taggers;
        }

        internal IEnumerable<Lazy<IViewTaggerProvider, IViewTaggerMetadata>> GetViewTaggersForType(IContentType type,
            Type taggerType)
        {
            var key = new ContentAndTypeData(type, taggerType);
            if (ViewTaggerProviderMap.TryGetValue(key, out var taggers))
                return taggers;
            taggers = new List<Lazy<IViewTaggerProvider, IViewTaggerMetadata>>(
                ViewTaggerProviders.Where(f => Match(type, taggerType, f.Metadata)));
            ImmutableInterlocked.Update(ref ViewTaggerProviderMap, s => s.Add(key, taggers));
            return taggers;
        }

        private static bool Match(IContentType bufferContentType, Type taggerType, ITaggerMetadata tagMetadata)
        {
            var flag = tagMetadata.ContentTypes.Any(bufferContentType.IsOfType);
            return flag && tagMetadata.TagTypes.Any(taggerType.IsAssignableFrom);
        }

        internal class ContentAndTypeData
        {
            public readonly IContentType ContentType;
            public readonly Type TaggerType;

            public ContentAndTypeData(IContentType contentType, Type taggerType)
            {
                ContentType = contentType;
                TaggerType = taggerType;
            }

            public override bool Equals(object obj)
            {
                if (obj is ContentAndTypeData contentAndTypeData && contentAndTypeData.ContentType == ContentType)
                    return contentAndTypeData.TaggerType == TaggerType;
                return false;
            }

            public override int GetHashCode()
            {
                return ContentType.GetHashCode() ^ TaggerType.GetHashCode();
            }
        }
    }
}