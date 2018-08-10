using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IContentTypeRegistryService))]
    internal sealed class ContentTypeRegistry : IContentTypeRegistryService
    {
        internal static readonly ContentType UnknownContentTypeImpl = new ContentType("UNKNOWN");
        private MapCollection _maps;

        [ImportMany]
        internal List<Lazy<ContentTypeDefinition, IContentTypeDefinitionMetadata>> ContentTypeDefinitions { get; set; }

        public IContentType UnknownContentType => UnknownContentTypeImpl;

        public IEnumerable<IContentType> ContentTypes
        {
            get
            {
                BuildContentTypes();
                return _maps.NameToContentTypeMap.Values;
            }
        }

        private void BuildContentTypes()
        {
            var comparand = Volatile.Read(ref _maps);
            if (comparand != null)
                return;
            var builder1 = MapCollection.Empty.NameToContentTypeMap.ToBuilder();
            var builder2 = MapCollection.Empty.MimeTypeToContentTypeMap.ToBuilder();
            builder1.Add("UNKNOWN", UnknownContentTypeImpl);
            foreach (var contentTypeDefinition in ContentTypeDefinitions)
                AddContentTypeFromMetadata(contentTypeDefinition.Metadata.Name, contentTypeDefinition.Metadata.MimeType, contentTypeDefinition.Metadata.BaseDefinition, builder1, builder2);
            var contentTypeImplList = new List<ContentType>(builder1.Count);
            contentTypeImplList.AddRange(builder1.Values);
            foreach (var contentTypeImpl in contentTypeImplList)
                contentTypeImpl.ProcessBaseTypes(builder1, builder2);
            foreach (var contentTypeImpl in builder1.Values)
                contentTypeImpl.CheckForCycle(true);
            var builder3 = MapCollection.Empty.FileExtensionToContentTypeMap.ToBuilder();
            var builder4 = MapCollection.Empty.FileNameToContentTypeMap.ToBuilder();

            Interlocked.CompareExchange(ref _maps,
                new MapCollection(builder1.ToImmutable(), builder2.ToImmutable(), builder3.ToImmutable(),
                    builder4.ToImmutable()), comparand);
        }

        private static string RemoveExtensionDot(string extension)
        {
            if (!extension.StartsWith("."))
                return extension;
            return extension.TrimStart('.');
        }

        public IContentType GetContentType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));
            BuildContentTypes();
            _maps.NameToContentTypeMap.TryGetValue(typeName, out var contentTypeImpl);
            return contentTypeImpl;
        }

        public IContentType AddContentType(string typeName, IEnumerable<string> baseTypeNames)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));
            if (GetContentType(typeName) != null)
                throw new ArgumentException();
            var comparand = Volatile.Read(ref _maps);
            while (true)
            {
                var pseudoBuilder1 = new PseudoBuilder<string, ContentType>(comparand.NameToContentTypeMap);
                var pseudoBuilder2 = new PseudoBuilder<string, ContentType>(comparand.MimeTypeToContentTypeMap);
                var contentType = ContentType.AddContentTypeFromMetadata(typeName, null, baseTypeNames, pseudoBuilder1, pseudoBuilder2);
                contentType.ProcessBaseTypes(pseudoBuilder1, pseudoBuilder2);
                if (!contentType.CheckForCycle(false))
                {
                    var map =
                        Interlocked.CompareExchange(ref _maps,
                            new MapCollection(pseudoBuilder1.Source, pseudoBuilder2.Source,
                                comparand.FileExtensionToContentTypeMap, comparand.FileNameToContentTypeMap),
                            comparand);
                    if (map != comparand)
                        comparand = map;
                    else
                        return contentType;
                }
                else
                    break;
            }
            throw new InvalidOperationException();
        }

        public void RemoveContentType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));
            BuildContentTypes();
            var comparand = Volatile.Read(ref _maps);
            while (true)
            {
                if (comparand.NameToContentTypeMap.TryGetValue(typeName, out var type))
                {
                    if (type != UnknownContentTypeImpl)
                    {
                        if (!IsBaseType(type, out _))
                        {
                            if (_maps.FileExtensionToContentTypeMap.Values.All(x => x != type))
                            {
                                if (_maps.FileNameToContentTypeMap.Values.Any(x => x == type))
                                {
                                    var mapCollection =
                                        Interlocked.CompareExchange(
                                            ref _maps,
                                            new MapCollection(
                                                comparand.NameToContentTypeMap.Remove(typeName),
                                                type.MimeType != null
                                                    ? comparand.MimeTypeToContentTypeMap.Remove(type.MimeType)
                                                    : comparand.MimeTypeToContentTypeMap,
                                                comparand.FileExtensionToContentTypeMap,
                                                comparand.FileNameToContentTypeMap), comparand);
                                    if (mapCollection != comparand)
                                        comparand = mapCollection;
                                }
                                else
                                    throw new InvalidOperationException();
                            }
                            else
                                throw new InvalidOperationException();
                        }
                        else
                            throw new InvalidOperationException();
                    }
                    else
                        throw new InvalidOperationException();
                }
                break;
            }
        }

        private bool IsBaseType(ContentType typeToCheck, out ContentType derivedType)
        {
            derivedType = null;
            foreach (var contentTypeImpl in _maps.NameToContentTypeMap.Values)
            {
                if (contentTypeImpl == typeToCheck) continue;
                if (contentTypeImpl.BaseTypes.All(baseType => baseType != typeToCheck))
                    continue;
                derivedType = contentTypeImpl;
                return true;
            }
            return false;
        }

        internal static ContentType AddContentTypeFromMetadata(string contentTypeName, string mimeType, IEnumerable<string> baseTypes, IDictionary<string, ContentType> nameToContentTypeBuilder, IDictionary<string, ContentType> mimeTypeToContentTypeBuilder)
        {
            if (string.IsNullOrEmpty(contentTypeName))
                return null;
            if (!nameToContentTypeBuilder.TryGetValue(contentTypeName, out var contentType))
            {
                var flag = false;
                if (string.IsNullOrWhiteSpace(mimeType))
                    mimeType = "text/x-" + contentTypeName.ToLowerInvariant();
                else if (mimeTypeToContentTypeBuilder.ContainsKey(mimeType))
                    mimeType = null;
                else
                    flag = true;
                contentType = new ContentType(contentTypeName, mimeType, baseTypes);
                nameToContentTypeBuilder.Add(contentTypeName, contentType);
                if (flag)
                    mimeTypeToContentTypeBuilder.Add(mimeType, contentType);
            }
            else
                contentType.AddUnprocessedBaseTypes(baseTypes);

            return contentType;
        }

        private class MapCollection
        {
            public static readonly MapCollection Empty = new MapCollection();
            public readonly ImmutableDictionary<string, ContentType> NameToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> MimeTypeToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> FileExtensionToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> FileNameToContentTypeMap;

            private MapCollection()
            {
                NameToContentTypeMap = ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
                MimeTypeToContentTypeMap = ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.Ordinal);
                FileExtensionToContentTypeMap = ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
                FileNameToContentTypeMap = ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
            }

            public MapCollection(ImmutableDictionary<string, ContentType> nameToContentType, ImmutableDictionary<string, ContentType> mimeTypeToContentTypeMap, ImmutableDictionary<string, ContentType> fileExtensionToContentTypeMap, ImmutableDictionary<string, ContentType> fileNameToContentTypeMap)
            {
                NameToContentTypeMap = nameToContentType;
                MimeTypeToContentTypeMap = mimeTypeToContentTypeMap;
                FileExtensionToContentTypeMap = fileExtensionToContentTypeMap;
                FileNameToContentTypeMap = fileNameToContentTypeMap;
            }
        }

        private class PseudoBuilder<K, V> : IDictionary<K, V>
        {
            public ImmutableDictionary<K, V> Source { get; private set; }

            public PseudoBuilder(ImmutableDictionary<K, V> source)
            {
                Source = source;
            }

            public void Add(K key, V value)
            {
                Source = Source.Add(key, value);
            }

            public bool ContainsKey(K key)
            {
                return Source.ContainsKey(key);
            }

            public bool TryGetValue(K key, out V value)
            {
                return Source.TryGetValue(key, out value);
            }

            public V this[K key]
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public ICollection<K> Keys => throw new NotImplementedException();

            public ICollection<V> Values => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(KeyValuePair<K, V> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<K, V> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(K key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<K, V> item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}