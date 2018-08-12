using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
{
    [Guid(CategoryGuids.EditorTextManager)]
    internal sealed class EditorTextManagerFontAndColorCategory : TextManagerFontAndColorCategory
    {
        public EditorTextManagerFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidEditorTextManager;
        }
    }
}