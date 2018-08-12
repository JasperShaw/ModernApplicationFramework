using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Guid(CategoryGuids.TextEditorGroup)]
    internal class TextEditorGroup : IFontAndColorGroup
    {
        public Guid GetCategory(int category)
        {
            var pguidCategory = Guid.Empty;
            switch (category)
            {
                //case 0:
                //    pguidCategory = CategoryGuids.GuidEditorLanguageService;
                //    break;
                case 1:
                    pguidCategory = CategoryGuids.GuidEditorMef;
                    break;
                case 2:
                    pguidCategory = CategoryGuids.GuidEditorTextManager;
                    break;
                case 3:
                    pguidCategory = CategoryGuids.GuidEditorTextMarker;
                    break;
            }
            return pguidCategory;
        }

        public int GetCount()
        {
            return 3;
        }

        public string GetGroupName()
        {
            //TODO: Localize
            return "Text-Editor";
            //return ResourceStrings.GetShellString(13983);
        }

        public FcPriority GetPriority()
        {
            return FcPriority.Editor;
        }
    }
}