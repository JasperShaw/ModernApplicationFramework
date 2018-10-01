using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal sealed class BraceCompletionAggregator : IBraceCompletionAggregator
    {
        private readonly BraceCompletionAggregatorFactory _factory;
        private Dictionary<char, List<IContentType>> _contentTypeCache;
        private Dictionary<char, Dictionary<IContentType, List<ProviderHelper>>> _providerCache;

        public string OpeningBraces { get; private set; }

        public string ClosingBraces { get; private set; }

        public BraceCompletionAggregator(BraceCompletionAggregatorFactory factory)
        {
            _factory = factory;
            Init();
        }

        public bool IsSupportedContentType(IContentType contentType, char openingBrace)
        {
            if (_contentTypeCache.TryGetValue(openingBrace, out var source))
                return source.Any(t => contentType.IsOfType(t.TypeName));
            return false;
        }

        public bool TryCreateSession(ITextView textView, SnapshotPoint openingPoint, char openingBrace,
            out IBraceCompletionSession session)
        {
            var contentType = openingPoint.Snapshot.TextBuffer.ContentType;
            if (_contentTypeCache.TryGetValue(openingBrace, out var contentTypeList))
            {
                foreach (var key in contentTypeList)
                {
                    if (contentType.IsOfType(key.TypeName))
                    {
                        if (_providerCache[openingBrace].TryGetValue(key, out var providerHelperList))
                        {
                            using (var enumerator = providerHelperList.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    var current = enumerator.Current;
                                    var completionSession = (IBraceCompletionSession)null;
                                    var factory = _factory;
                                    var textView1 = textView;
                                    var openingPoint1 = openingPoint;
                                    var num = (int)openingBrace;
                                    ref var local = ref completionSession;
                                    if (current.TryCreate(factory, textView1, openingPoint1, (char)num, out local))
                                    {
                                        session = completionSession;
                                        return true;
                                    }
                                }
                            }
                        }

                        break;
                    }
                }
            }
            session = null;
            return false;
        }

        private void Init()
        {
            var charSet = new HashSet<char>();
            _providerCache = new Dictionary<char, Dictionary<IContentType, List<ProviderHelper>>>();
            _contentTypeCache = new Dictionary<char, List<IContentType>>();
            var providerHelperList1 = new List<ProviderHelper>();

            providerHelperList1.AddRange(_factory.SessionProviders.Select(p => new ProviderHelper(p)));
            providerHelperList1.AddRange(_factory.ContextProviders.Select(p => new ProviderHelper(p)));
            providerHelperList1.AddRange(_factory.DefaultProviders.Select(p => new ProviderHelper(p)));

            foreach (var providerHelper in providerHelperList1)
            {
                foreach (var closingBrace in providerHelper.Metadata.ClosingBraces)
                    charSet.Add(closingBrace);
                foreach (var openingBrace in providerHelper.Metadata.OpeningBraces)
                {
                    if (!_providerCache.TryGetValue(openingBrace, out var dictionary))
                    {
                        dictionary = new Dictionary<IContentType, List<ProviderHelper>>();
                        _providerCache.Add(openingBrace, dictionary);
                    }

                    foreach (var key in providerHelper.Metadata.ContentTypes.Select(typeName => _factory.ContentTypeRegistryService.GetContentType(typeName)).Where(t => t != null))
                    {
                        if (!dictionary.TryGetValue(key, out var providerHelperList2))
                        {
                            providerHelperList2 = new List<ProviderHelper>();
                            dictionary.Add(key, providerHelperList2);
                        }
                        providerHelperList2.Add(providerHelper);
                    }
                }
            }

            OpeningBraces = string.Join(string.Empty, _providerCache.Keys);
            ClosingBraces = string.Join(string.Empty, charSet);
            foreach (var keyValuePair in _providerCache)
            {
                _contentTypeCache.Add(keyValuePair.Key, SortContentTypes(keyValuePair.Value.Keys.ToList()));
                foreach (var key in keyValuePair.Value.Keys)
                    keyValuePair.Value[key].Sort();
            }
        }

        private static List<IContentType> SortContentTypes(IReadOnlyCollection<IContentType> contentTypes)
        {
            var contentTypeList = new List<IContentType>(contentTypes.Count);
            foreach (var contentType in contentTypes)
            {
                var flag = false;
                for (var index = 0; index < contentTypeList.Count; ++index)
                {
                    if (contentType.IsOfType(contentTypeList[index].TypeName))
                    {
                        contentTypeList.Insert(index, contentType);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    contentTypeList.Add(contentType);
            }
            return contentTypeList;
        }

        private static char GetClosingBrace(IBraceCompletionMetadata metadata, char openingBrace)
        {
            var enumerator1 = metadata.OpeningBraces.GetEnumerator();
            var enumerator2 = metadata.ClosingBraces.GetEnumerator();
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                if (enumerator1.Current == openingBrace)
                    return enumerator2.Current;
            }
            return ' ';
        }

        private static bool AllowDefaultSession(SnapshotPoint openingPoint, char openingBrace, char closingBrace)
        {
            if (openingBrace == closingBrace && openingPoint.Position > 0)
            {
                var ch = openingPoint.Subtract(1).GetChar();
                if (openingBrace.Equals(ch))
                    return false;
            }
            return true;
        }

        private class ProviderHelper : IComparable<ProviderHelper>
        {
            private Lazy<IBraceCompletionSessionProvider, IBraceCompletionMetadata> _sessionPair;
            private Lazy<IBraceCompletionContextProvider, IBraceCompletionMetadata> _contextPair;
            private Lazy<IBraceCompletionDefaultProvider, IBraceCompletionMetadata> _defaultPair;

            public ProviderHelper(Lazy<IBraceCompletionSessionProvider, IBraceCompletionMetadata> sessionPair)
            {
                _sessionPair = sessionPair;
            }

            public ProviderHelper(Lazy<IBraceCompletionContextProvider, IBraceCompletionMetadata> contextPair)
            {
                _contextPair = contextPair;
            }

            public ProviderHelper(Lazy<IBraceCompletionDefaultProvider, IBraceCompletionMetadata> defaultPair)
            {
                _defaultPair = defaultPair;
            }

            public bool IsSession => _sessionPair != null;

            public bool IsContext => _contextPair != null;

            public bool IsDefault => _defaultPair != null;

            public IBraceCompletionMetadata Metadata
            {
                get
                {
                    if (IsSession)
                        return _sessionPair.Metadata;
                    return IsContext ? _contextPair.Metadata : _defaultPair.Metadata;
                }
            }

            public bool TryCreate(BraceCompletionAggregatorFactory factory, ITextView textView, SnapshotPoint openingPoint, char openingBrace, out IBraceCompletionSession session)
            {
                char closingBrace = GetClosingBrace(Metadata, openingBrace);
                if (IsSession)
                {
                    bool created = false;
                    IBraceCompletionSession currentSession = null;
                    factory.GuardedOperations.CallExtensionPoint(() => created = _sessionPair.Value.TryCreateSession(textView, openingPoint, openingBrace, closingBrace, out currentSession));
                    if (created)
                    {
                        session = currentSession;
                        return true;
                    }
                    session = null;
                    return false;
                }
                if (IsContext)
                {
                    IBraceCompletionContext context = null;
                    if (AllowDefaultSession(openingPoint, openingBrace, closingBrace))
                    {
                        bool created = false;
                        factory.GuardedOperations.CallExtensionPoint(() => created = _contextPair.Value.TryCreateContext(textView, openingPoint, openingBrace, closingBrace, out context));
                        if (created)
                        {
                            session = new BraceCompletionDefaultSession(textView, openingPoint.Snapshot.TextBuffer, openingPoint, openingBrace, closingBrace, /*factory.UndoManager,*/ factory.EditorOperationsFactoryService, context);
                            return true;
                        }
                    }
                    session = null;
                    return false;
                }
                if (IsDefault && AllowDefaultSession(openingPoint, openingBrace, closingBrace))
                {
                    session = new BraceCompletionDefaultSession(textView, openingPoint.Snapshot.TextBuffer, openingPoint, openingBrace, closingBrace, /*factory.UndoManager,*/ factory.EditorOperationsFactoryService);
                    return true;
                }
                session = null;
                return false;
            }

            public int CompareTo(ProviderHelper other)
            {
                if (IsSession && !other.IsSession)
                    return -1;
                if (other.IsSession)
                    return 1;
                if (IsContext && !other.IsContext)
                    return -1;
                return other.IsContext ? 1 : 0;
            }
        }
    }
}
