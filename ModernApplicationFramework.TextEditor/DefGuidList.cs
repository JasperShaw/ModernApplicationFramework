using System;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    public static class DefGuidList
    {
        public static readonly Guid GuidEditPropCategoryViewMasterSettings = new Guid("{AE21345B-6C76-496F-8BAE-F33B20FA761A}");
        public static readonly Guid OutputWindowCategory = new Guid(CategoryGuids.OutputWindow);
        public static readonly Guid TextEditorCategory = new Guid(CategoryGuids.EditorTextManager);
        public static readonly Guid CommandWindowCategory = new Guid(CategoryGuids.CommandWindow);
        public static readonly Guid ImmediateWindowCategory = new Guid(CategoryGuids.ImmediateWindow);
        public static readonly Guid FindResultsWindowCategory = new Guid(CategoryGuids.FindResultsWindow);
    }
}