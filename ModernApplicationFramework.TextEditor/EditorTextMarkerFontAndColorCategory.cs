using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
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