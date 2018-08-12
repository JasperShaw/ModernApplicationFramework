using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    [Export(typeof(IContentTypeRegistryService))]
    [Export(typeof(IFileExtensionRegistryService))]
    internal sealed class ContentTypeRegistry : IContentTypeRegistryService, IFileExtensionRegistryService
    {
        internal static readonly ContentType UnknownContentTypeImpl = new ContentType("UNKNOWN");
        private MapCollection _maps;

        public IEnumerable<IContentType> ContentTypes
        {
            get
            {
                BuildContentTypes();
                return _maps.NameToContentTypeMap.Values;
            }
        }

        public IContentType UnknownContentType => UnknownContentTypeImpl;

        [ImportMany]
        internal List<Lazy<ContentTypeDefinition, IContentTypeDefinitionMetadata>> ContentTypeDefinitions { get; set; }

        [ImportMany]
        internal List<Lazy<FileExtensionToContentTypeDefinition, IFileToContentTypeMetadata>>
            FileToContentTypeProductions { get; set; }

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
                var contentType =
                    ContentType.AddContentTypeFromMetadata(typeName, null, baseTypeNames, pseudoBuilder1,
                        pseudoBuilder2);
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
                {
                    break;
                }
            }

            throw new InvalidOperationException();
        }

        public void AddFileExtension(string extension, IContentType contentType)
        {
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException(nameof(extension));
            var contentTypeImpl1 = contentType as ContentType;
            if (contentTypeImpl1 == null || contentTypeImpl1 == UnknownContentTypeImpl)
                throw new ArgumentException(nameof(contentType));
            BuildContentTypes();
            extension = RemoveExtensionDot(extension);
            ContentType contentTypeImpl2;
            MapCollection mapCollection;
            for (var comparand = Volatile.Read(ref _maps);
                !comparand.FileExtensionToContentTypeMap.TryGetValue(extension, out contentTypeImpl2);
                comparand = mapCollection)
            {
                mapCollection = Interlocked.CompareExchange(ref _maps,
                    new MapCollection(comparand.NameToContentTypeMap, comparand.MimeTypeToContentTypeMap,
                        comparand.FileExtensionToContentTypeMap.Add(extension, contentTypeImpl1),
                        comparand.FileNameToContentTypeMap), comparand);
                if (mapCollection == comparand)
                    return;
            }

            //TODO: Text
            if (contentTypeImpl2 != contentTypeImpl1)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture,
                    "No multiple content type: {0}", extension));
        }

        public void AddFileName(string fileName, IContentType contentType)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException(nameof(fileName));
            var contentTypeImpl1 = contentType as ContentType;
            if (contentTypeImpl1 == null || contentTypeImpl1 == UnknownContentTypeImpl)
                throw new ArgumentException(nameof(contentType));
            BuildContentTypes();
            ContentType contentTypeImpl2;
            MapCollection mapCollection;
            for (var comparand = Volatile.Read(ref _maps);
                !comparand.FileNameToContentTypeMap.TryGetValue(fileName, out contentTypeImpl2);
                comparand = mapCollection)
            {
                mapCollection = Interlocked.CompareExchange(ref _maps,
                    new MapCollection(comparand.NameToContentTypeMap, comparand.MimeTypeToContentTypeMap,
                        comparand.FileExtensionToContentTypeMap,
                        comparand.FileNameToContentTypeMap.Add(fileName, contentTypeImpl1)), comparand);
                if (mapCollection == comparand)
                    return;
            }

            //TODO: Text
            if (contentTypeImpl2 != contentTypeImpl1)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture,
                    "No multiple content type: {0}", fileName));
        }

        public IContentType GetContentType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException(nameof(typeName));
            BuildContentTypes();
            _maps.NameToContentTypeMap.TryGetValue(typeName, out var contentTypeImpl);
            return contentTypeImpl;
        }

        public IContentType GetContentTypeForExtension(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            BuildContentTypes();
            _maps.FileExtensionToContentTypeMap.TryGetValue(RemoveExtensionDot(extension), out var contentTypeImpl);
            return contentTypeImpl ?? UnknownContentTypeImpl;
        }

        public IContentType GetContentTypeForFileName(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            BuildContentTypes();
            _maps.FileNameToContentTypeMap.TryGetValue(fileName, out var contentTypeImpl);
            return contentTypeImpl ?? UnknownContentTypeImpl;
        }

        public IContentType GetContentTypeForFileNameOrExtension(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            var contentTypeForFileName = GetContentTypeForFileName(name);
            if (contentTypeForFileName == UnknownContentTypeImpl)
            {
                var extension = Path.GetExtension(name);
                if (!string.IsNullOrEmpty(extension))
                    return GetContentTypeForExtension(extension);
            }

            return contentTypeForFileName;
        }

        public IEnumerable<string> GetExtensionsForContentType(IContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            BuildContentTypes();
            foreach (var extensionToContentType in _maps.FileExtensionToContentTypeMap)
                if (contentType == extensionToContentType.Value)
                    yield return extensionToContentType.Key;
        }

        public IEnumerable<string> GetFileNamesForContentType(IContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            BuildContentTypes();
            foreach (var nameToContentType in _maps.FileNameToContentTypeMap)
                if (contentType == nameToContentType.Value)
                    yield return nameToContentType.Key;
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
                                {
                                    throw new InvalidOperationException();
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                break;
            }
        }

        public void RemoveFileExtension(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));
            BuildContentTypes();
            extension = RemoveExtensionDot(extension);
            MapCollection mapCollection;
            for (var comparand = Volatile.Read(ref _maps);
                comparand.FileExtensionToContentTypeMap.ContainsKey(extension);
                comparand = mapCollection)
            {
                mapCollection = Interlocked.CompareExchange(ref _maps,
                    new MapCollection(comparand.NameToContentTypeMap, comparand.MimeTypeToContentTypeMap,
                        comparand.FileExtensionToContentTypeMap.Remove(extension), comparand.FileNameToContentTypeMap),
                    comparand);
                if (mapCollection == comparand)
                    break;
            }
        }

        public void RemoveFileName(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            BuildContentTypes();
            MapCollection mapCollection;
            for (var comparand = Volatile.Read(ref _maps);
                comparand.FileNameToContentTypeMap.ContainsKey(fileName);
                comparand = mapCollection)
            {
                mapCollection = Interlocked.CompareExchange(ref _maps,
                    new MapCollection(comparand.NameToContentTypeMap, comparand.MimeTypeToContentTypeMap,
                        comparand.FileExtensionToContentTypeMap, comparand.FileNameToContentTypeMap.Remove(fileName)),
                    comparand);
                if (mapCollection == comparand)
                    break;
            }
        }

        internal static ContentType AddContentTypeFromMetadata(string contentTypeName, string mimeType,
            IEnumerable<string> baseTypes, IDictionary<string, ContentType> nameToContentTypeBuilder,
            IDictionary<string, ContentType> mimeTypeToContentTypeBuilder)
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
            {
                contentType.AddUnprocessedBaseTypes(baseTypes);
            }

            return contentType;
        }

        private static string RemoveExtensionDot(string extension)
        {
            if (!extension.StartsWith("."))
                return extension;
            return extension.TrimStart('.');
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
                AddContentTypeFromMetadata(contentTypeDefinition.Metadata.Name, contentTypeDefinition.Metadata.MimeType,
                    contentTypeDefinition.Metadata.BaseDefinition, builder1, builder2);
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

        private class MapCollection
        {
            public static readonly MapCollection Empty = new MapCollection();
            public readonly ImmutableDictionary<string, ContentType> FileExtensionToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> FileNameToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> MimeTypeToContentTypeMap;
            public readonly ImmutableDictionary<string, ContentType> NameToContentTypeMap;

            public MapCollection(ImmutableDictionary<string, ContentType> nameToContentType,
                ImmutableDictionary<string, ContentType> mimeTypeToContentTypeMap,
                ImmutableDictionary<string, ContentType> fileExtensionToContentTypeMap,
                ImmutableDictionary<string, ContentType> fileNameToContentTypeMap)
            {
                NameToContentTypeMap = nameToContentType;
                MimeTypeToContentTypeMap = mimeTypeToContentTypeMap;
                FileExtensionToContentTypeMap = fileExtensionToContentTypeMap;
                FileNameToContentTypeMap = fileNameToContentTypeMap;
            }

            private MapCollection()
            {
                NameToContentTypeMap =
                    ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
                MimeTypeToContentTypeMap =
                    ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.Ordinal);
                FileExtensionToContentTypeMap =
                    ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
                FileNameToContentTypeMap =
                    ImmutableDictionary<string, ContentType>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);
            }
        }

        private class PseudoBuilder<K, V> : IDictionary<K, V>
        {
            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public ICollection<K> Keys => throw new NotImplementedException();
            public ImmutableDictionary<K, V> Source { get; private set; }

            public ICollection<V> Values => throw new NotImplementedException();

            public PseudoBuilder(ImmutableDictionary<K, V> source)
            {
                Source = source;
            }

            public V this[K key]
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public void Add(K key, V value)
            {
                Source = Source.Add(key, value);
            }

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

            public bool ContainsKey(K key)
            {
                return Source.ContainsKey(key);
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

            public bool TryGetValue(K key, out V value)
            {
                return Source.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}