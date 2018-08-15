using System;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    public partial class OverviewScrollBarMarginControl
    {
        private readonly OverviewElement _overviewElement;

        public event EventHandler TrackSpanChanged;

        public IScrollMap Map => _overviewElement.Map;

        public double ThumbHeight => _overviewElement.ThumbHeight;

        public double TrackSpanTop => _overviewElement.TranslatePoint(new Point(0.0, 0.0), this).Y;

        public double TrackSpanBottom => _overviewElement.TranslatePoint(new Point(0.0, _overviewElement.ActualHeight), this).Y;

        public double TrackSpanHeight => _overviewElement.TrackSpanHeight;

        internal OverviewScrollBarMarginControl(OverviewElement overviewElement)
        {
            InitializeComponent();
            _overviewElement = overviewElement;
            SetRow(overviewElement, 1);
            Children.Add(overviewElement);
            _overviewElement.TrackSpanChanged += OnTrackSpanChanged;
            UpButton.SizeChanged += OnButtonSizeChanged;
            DownButton.SizeChanged += OnButtonSizeChanged;
        }

        public double GetYCoordinateOfBufferPosition(SnapshotPoint bufferPosition)
        {
            return _overviewElement.TranslatePoint(new Point(0.0, _overviewElement.GetYCoordinateOfBufferPosition(bufferPosition)), this).Y;
        }

        public double GetYCoordinateOfScrollMapPosition(double scrollMapPosition)
        {
            return _overviewElement.TranslatePoint(new Point(0.0, _overviewElement.GetYCoordinateOfScrollMapPosition(scrollMapPosition)), this).Y;
        }

        public SnapshotPoint GetBufferPositionOfYCoordinate(double y)
        {
            return _overviewElement.GetBufferPositionOfYCoordinate(TranslatePoint(new Point(0.0, y), _overviewElement).Y);
        }

        private void OnButtonSizeChanged(object sender, SizeChangedEventArgs e)
        {
            EventHandler trackSpanChanged = TrackSpanChanged;
            trackSpanChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTrackSpanChanged(object sender, EventArgs e)
        {
            EventHandler trackSpanChanged = TrackSpanChanged;
            trackSpanChanged?.Invoke(this, e);
        }
    }
}
