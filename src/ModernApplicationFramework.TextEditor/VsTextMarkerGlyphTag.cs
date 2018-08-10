namespace ModernApplicationFramework.TextEditor
{
    internal class VsTextMarkerGlyphTag : IGlyphTag
    {
        public VsTextMarkerTag TextMarkerTag { get; }

        public VsTextMarkerGlyphTag(VsTextMarkerTag textMarkerTag)
        {
            TextMarkerTag = textMarkerTag;
        }
    }
}