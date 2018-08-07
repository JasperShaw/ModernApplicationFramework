using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
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