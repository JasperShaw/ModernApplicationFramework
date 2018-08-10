using System;

namespace ModernApplicationFramework.Text.Logic.Tagging
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