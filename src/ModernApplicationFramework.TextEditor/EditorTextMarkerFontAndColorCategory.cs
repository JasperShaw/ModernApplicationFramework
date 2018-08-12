using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    [Guid(CategoryGuids.EditorTextMarker)]
    internal sealed class EditorTextMarkerFontAndColorCategory : TextMarkerFontAndColorCategory
    {
        public EditorTextMarkerFontAndColorCategory()
        {
            this.CategoryGuid = CategoryGuids.GuidEditorTextMarker;
        }
    }
}