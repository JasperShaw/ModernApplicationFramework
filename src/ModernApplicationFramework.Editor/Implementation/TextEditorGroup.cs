using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontAndColorGroup))]
    internal class TextEditorGroup : IFontAndColorGroup
    {
        public Guid GroupGuid => CategoryGuids.GuidTextEditorGroup;

        public Guid GetCategory(int category)
        {
            var pguidCategory = Guid.Empty;
            switch (category)
            {
                //case 0:
                //    pguidCategory = CategoryGuids.GuidEditorLanguageService;
                //    break;
                case 0:
                    pguidCategory = CategoryGuids.GuidEditorMef;
                    break;
                case 1:
                    pguidCategory = CategoryGuids.GuidEditorTextManager;
                    break;
                case 2:
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

        public ushort GetPriority()
        {
            return 0;
        }
    }
}