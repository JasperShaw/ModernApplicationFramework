using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
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