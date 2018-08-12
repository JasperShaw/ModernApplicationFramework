using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Implementation
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