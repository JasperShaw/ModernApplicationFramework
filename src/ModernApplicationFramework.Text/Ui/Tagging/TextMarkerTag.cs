using System;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class TextMarkerTag : ITextMarkerTag
    {
        public string Type { get; }

        public TextMarkerTag(string type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}