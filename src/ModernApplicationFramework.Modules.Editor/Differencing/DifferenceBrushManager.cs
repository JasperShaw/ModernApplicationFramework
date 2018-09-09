using System;
using System.Linq;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceBrushManager
    {
        internal static readonly SolidColorBrush DefaultRemovedLineBrush = Brushes.PaleVioletRed;
        internal static readonly SolidColorBrush DefaultAddedLineBrush = Brushes.LightYellow;
        internal static readonly SolidColorBrush DefaultRemovedWordBrush = Brushes.Red;
        internal static readonly SolidColorBrush DefaultAddedWordBrush = Brushes.Yellow;
        private readonly IEditorFormatMap _formatMap;

        public static DifferenceBrushManager GetBrushManager(ITextView view, IEditorFormatMapService formatMapService)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new DifferenceBrushManager(view, formatMapService.GetEditorFormatMap(view)));
        }

        public Brush RemovedLineBrush { get; private set; }

        public Brush AddedLineBrush { get; private set; }

        public Brush RemovedWordBrush { get; private set; }

        public Brush RemovedWordForegroundBrush { get; private set; }

        public Pen RemovedWordForegroundPen { get; private set; }

        public Brush AddedWordBrush { get; private set; }

        public Brush AddedWordForegroundBrush { get; private set; }

        public Pen AddedWordForegroundPen { get; private set; }

        public Brush ViewportBrush { get; private set; }

        public Pen ViewportPen { get; private set; }

        public Brush OverviewBrush { get; private set; }

        public event EventHandler<EventArgs> BrushesChanged;

        internal DifferenceBrushManager(ITextView view, IEditorFormatMap formatMap)
        {
            _formatMap = formatMap;
            InitializeBrushes();
            _formatMap.FormatMappingChanged += FormatMapChanged;
            view.Closed += (s, a) => _formatMap.FormatMappingChanged -= FormatMapChanged;
        }

        private void InitializeBrushes()
        {
            RemovedLineBrush = GetBrushValue("deltadiff.remove.line", DefaultRemovedLineBrush);
            RemovedWordBrush = GetBrushValue("deltadiff.remove.word", DefaultRemovedWordBrush);
            RemovedWordForegroundBrush = GetBrushValue("deltadiff.remove.word", DefaultRemovedWordBrush, "Foreground");
            RemovedWordForegroundPen = new Pen(RemovedWordForegroundBrush, 2.0);
            RemovedWordForegroundPen.Freeze();
            AddedLineBrush = GetBrushValue("deltadiff.add.line", DefaultAddedLineBrush);
            AddedWordBrush = GetBrushValue("deltadiff.add.word", DefaultAddedWordBrush);
            AddedWordForegroundBrush = GetBrushValue("deltadiff.add.word", DefaultAddedWordBrush, "Foreground");
            AddedWordForegroundPen = new Pen(AddedWordForegroundBrush, 2.0);
            AddedWordForegroundPen.Freeze();
            ViewportBrush = GetBrushValue("deltadiff.overview.color", Brushes.DarkGray, "Foreground");
            ViewportPen = new Pen(ViewportBrush, 2.0);
            ViewportPen.Freeze();
            OverviewBrush = GetBrushValue("deltadiff.overview.color", Brushes.Gray);
            // ISSUE: reference to a compiler-generated field
            var brushesChanged = BrushesChanged;
            brushesChanged?.Invoke(this, EventArgs.Empty);
        }

        private Brush GetBrushValue(string formatName, Brush defaultValue, string resource = "Background")
        {
            var properties = _formatMap.GetProperties(formatName);
            if (properties != null && properties.Contains(resource))
            {
                if (properties[resource] is Brush brush)
                    return brush;
            }
            return defaultValue;
        }

        private void FormatMapChanged(object sender, FormatItemsEventArgs e)
        {
            var changedItems = e.ChangedItems;

            bool Func(string item)
            {
                if (!string.Equals(item, "deltadiff.add.word", StringComparison.OrdinalIgnoreCase) && !string.Equals(item, "deltadiff.add.line", StringComparison.OrdinalIgnoreCase) && (!string.Equals(item, "deltadiff.remove.word", StringComparison.OrdinalIgnoreCase) && !string.Equals(item, "deltadiff.remove.line", StringComparison.OrdinalIgnoreCase))) return string.Equals(item, "deltadiff.overview.color", StringComparison.OrdinalIgnoreCase);
                return true;
            }

            if (!changedItems.Any(Func))
                return;
            InitializeBrushes();
        }
    }
}