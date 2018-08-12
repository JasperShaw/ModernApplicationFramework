using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Guid(CategoryGuids.EditorTextMarker)]
    internal sealed class EditorTextMarkerFontAndColorCategory : TextMarkerFontAndColorCategory
    {
        public EditorTextMarkerFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidEditorTextMarker;
        }
    }
}