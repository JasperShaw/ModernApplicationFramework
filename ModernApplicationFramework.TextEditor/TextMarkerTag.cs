using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextMarkerTag : ITextMarkerTag
    {
        public TextMarkerTag(string type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Type { get; }
    }
}