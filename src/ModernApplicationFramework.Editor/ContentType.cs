using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor
{
    internal class ContentType : IContentType
    {
        private static readonly ContentType[] EmptyBaseTypes = Array.Empty<ContentType>();
        private ContentType[] _baseTypeList = EmptyBaseTypes;
        private VisitState _state = VisitState.Visited;

        public string TypeName { get; }
        public string DisplayName { get; }

        public string MimeType { get; }

        internal IEnumerable<string> UnprocessedBaseTypes;

        public ContentType(string name, string mimeType = null, IEnumerable<string> baseTypes = null)
        {
            DisplayName = name;
            TypeName = name;
            MimeType = mimeType;
            UnprocessedBaseTypes = baseTypes;
        }

        public bool IsOfType(string type)
        {
            return string.Compare(type, DisplayName, StringComparison.OrdinalIgnoreCase) == 0 ||
                   _baseTypeList.Any(t => t.IsOfType(type));
        }

        
        public IEnumerable<IContentType> BaseTypes => _baseTypeList;

        internal static ContentType AddContentTypeFromMetadata(string contentTypeName, string mimeType, IEnumerable<string> baseTypes, 
            IDictionary<string, ContentType> nameToContentTypeBuilder, IDictionary<string, ContentType> mimeTypeToContentTypeBuilder)
        {
            if (string.IsNullOrEmpty(contentTypeName))
                return null;
            if (!nameToContentTypeBuilder.TryGetValue(contentTypeName, out ContentType contentTypeImpl))
            {
                bool flag = false;
                if (string.IsNullOrWhiteSpace(mimeType))
                    mimeType = "text/x-" + contentTypeName.ToLowerInvariant();
                else if (mimeTypeToContentTypeBuilder.ContainsKey(mimeType))
                    mimeType = null;
                else
                    flag = true;
                contentTypeImpl = new ContentType(contentTypeName, mimeType, baseTypes);
                nameToContentTypeBuilder.Add(contentTypeName, contentTypeImpl);
                if (flag)
                    mimeTypeToContentTypeBuilder.Add(mimeType, contentTypeImpl);
            }
            else
                contentTypeImpl.AddUnprocessedBaseTypes(baseTypes);
            return contentTypeImpl;
        }

        internal void AddUnprocessedBaseTypes(IEnumerable<string> newBaseTypes)
        {
            if (newBaseTypes == null)
                return;
            if (UnprocessedBaseTypes.Equals(EmptyBaseTypes))
            {
                UnprocessedBaseTypes = newBaseTypes;
            }
            else
            {
                var stringList = new List<string>(UnprocessedBaseTypes);
                stringList.AddRange(newBaseTypes);
                UnprocessedBaseTypes = stringList;
            }
        }

        internal void ProcessBaseTypes(IDictionary<string, ContentType> nameToContentTypeBuilder, IDictionary<string, ContentType> mimeTypeToContentTypeBuilder)
        {
            if (UnprocessedBaseTypes == null)
                return;
            var contentTypeImplList = new List<ContentType>();
            foreach (var unprocessedBaseType in UnprocessedBaseTypes)
            {
                var contentTypeImpl = ContentTypeRegistry.AddContentTypeFromMetadata(unprocessedBaseType, null,
                    null, nameToContentTypeBuilder, mimeTypeToContentTypeBuilder);
                if (contentTypeImpl == ContentTypeRegistry.UnknownContentTypeImpl)
                    throw new InvalidOperationException();
                if (!contentTypeImplList.Contains(contentTypeImpl))
                    contentTypeImplList.Add(contentTypeImpl);
            }
            if (contentTypeImplList.Count > 0)
            {
                _baseTypeList = contentTypeImplList.ToArray();
                _state = VisitState.NotVisited;
            }
            UnprocessedBaseTypes = null;
        }

        internal bool CheckForCycle(bool breakCycle)
        {
            try
            {
                if (_baseTypeList.Length != 0)
                {
                    _state = VisitState.Visiting;
                    foreach (var baseType in _baseTypeList)
                    {
                        if (baseType._state == VisitState.Visiting)
                        {
                            if (breakCycle)
                                _baseTypeList = EmptyBaseTypes;
                            return true;
                        }
                        if (baseType._state == VisitState.NotVisited && baseType.CheckForCycle(breakCycle))
                            return true;
                    }
                }
            }
            finally
            {
                _state = VisitState.Visited;
            }
            return false;
        }

        internal enum VisitState
        {
            NotVisited,
            Visiting,
            Visited,
        }
    }
}
