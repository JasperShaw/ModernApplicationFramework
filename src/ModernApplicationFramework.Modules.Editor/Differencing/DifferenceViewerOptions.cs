using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    public static class DifferenceViewerOptions
    {
        public static readonly EditorOptionKey<DifferenceViewMode> ViewModeId = new EditorOptionKey<DifferenceViewMode>(ViewModeName);
        public static readonly EditorOptionKey<DifferenceHighlightMode> HighlightModeId = new EditorOptionKey<DifferenceHighlightMode>(HighlightModeName);
        public static readonly EditorOptionKey<bool> ScrollToFirstDiffId = new EditorOptionKey<bool>(ScrollToFirstDiffName);
        public static readonly EditorOptionKey<bool> SynchronizeSideBySideViewsId = new EditorOptionKey<bool>(SynchronizeSideBySideViewsName);
        public const string ViewModeName = "Diff/View/ViewMode";
        public const string HighlightModeName = "Diff/View/HighlightMode";
        public const string ScrollToFirstDiffName = "Diff/View/ScrollToFirstDiff";
        public const string SynchronizeSideBySideViewsName = "Diff/View/SynchronizeSideBySideViews";
    }
}