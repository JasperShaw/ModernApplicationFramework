using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.TextEditor;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class StableContentTypeComparer : IComparer<IEnumerable<string>>
    {
        private readonly IContentTypeRegistryService _contentTypeRegistryService;

        public StableContentTypeComparer(IContentTypeRegistryService contentTypeRegistryService)
        {
            var typeRegistryService = contentTypeRegistryService;
            _contentTypeRegistryService = typeRegistryService ?? throw new ArgumentNullException(nameof(contentTypeRegistryService));
        }

        public int Compare(IEnumerable<string> x, IEnumerable<string> y)
        {
            if (x.SequenceEqual(y))
                return 0;
            if ((from typeName in x select _contentTypeRegistryService.GetContentType(typeName) into contentType where contentType != null from type in y where contentType.IsOfType(type) select contentType).Any())
            {
                return -1;
            }
            foreach (var typeName in y)
            {
                var contentType = _contentTypeRegistryService.GetContentType(typeName);
                if (contentType != null)
                {
                    foreach (var type in x)
                    {
                        if (contentType.IsOfType(type))
                            return 1;
                    }
                }
            }
            return 0;
        }
    }
}