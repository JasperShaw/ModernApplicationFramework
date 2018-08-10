using System.Runtime.InteropServices;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    [Guid(CategoryGuids.EditorMef)]
    internal sealed class EditorMefFontAndColorCategory : MefFontAndColorCategory
    {
        public EditorMefFontAndColorCategory()
        {
            CategoryGuid = CategoryGuids.GuidEditorMef;
        }
    }
}