using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
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