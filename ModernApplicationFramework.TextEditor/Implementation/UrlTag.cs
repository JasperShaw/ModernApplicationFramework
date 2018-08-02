using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public class UrlTag : IUrlTag
    {
        public Uri Url { get; }

        public UrlTag(Uri url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}