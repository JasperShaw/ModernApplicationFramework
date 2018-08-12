namespace ModernApplicationFramework.Editor
{
    internal class TextMarkerGlyphTag : IGlyphTag
    {
        public VsTextMarkerTag TextMarkerTag { get; }

        public TextMarkerGlyphTag(VsTextMarkerTag textMarkerTag)
        {
            TextMarkerTag = textMarkerTag;
        }
    }
}