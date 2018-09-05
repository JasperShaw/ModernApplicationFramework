using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading.WaitDialog
{
    internal class WaitDialogDataSource : ObservableObject
    {
        private string _caption;
        private string _waitMessage;
        private string _progressMessage;
        private bool _isProgressVisible;
        private bool _showMarqueeProgress;
        private bool _isCancellable;
        private int _currentStep;
        private int _totalSteps;

        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value, nameof(Caption));
        }

        public string WaitMessage
        {
            get => _waitMessage;
            set => SetProperty(ref _waitMessage, value, nameof(WaitMessage));
        }

        public string ProgressMessage
        {
            get => _progressMessage;
            set => SetProperty(ref _progressMessage, value, nameof(ProgressMessage));
        }

        public bool IsProgressVisible
        {
            get => _isProgressVisible;
            set => SetProperty(ref _isProgressVisible, value, nameof(IsProgressVisible));
        }

        public bool ShowMarqueeProgress
        {
            get => _showMarqueeProgress;
            set => SetProperty(ref _showMarqueeProgress, value, nameof(ShowMarqueeProgress));
        }

        public bool IsCancellable
        {
            get => _isCancellable;
            set => SetProperty(ref _isCancellable, value, nameof(IsCancellable));
        }

        public int CurrentStep
        {
            get => _currentStep;
            set => SetProperty(ref _currentStep, value, nameof(CurrentStep));
        }

        public int TotalSteps
        {
            get => _totalSteps;
            set => SetProperty(ref _totalSteps, value, nameof(TotalSteps));
        }
    }
}
