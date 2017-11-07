using System;

namespace ModernApplicationFramework.Utilities
{
    public struct ImageInfo
    {
        public Uri Path { get; }

        public string Id { get; }

        public bool? FromXamlResource { get; }

        public ImageInfo(string id, string path, bool fromXaml)
        {
            Id = id;
            Path = new Uri(path, UriKind.RelativeOrAbsolute);
            FromXamlResource = fromXaml;
        }

        public ImageInfo(string path)
        {
            Id = string.Empty;
            Path = new Uri(path, UriKind.RelativeOrAbsolute);
            FromXamlResource = false;
        }
    }
}