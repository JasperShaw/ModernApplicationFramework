using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Editor.Implementation
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