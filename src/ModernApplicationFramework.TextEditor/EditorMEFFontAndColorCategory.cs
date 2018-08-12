using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Implementation;

namespace ModernApplicationFramework.Editor
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