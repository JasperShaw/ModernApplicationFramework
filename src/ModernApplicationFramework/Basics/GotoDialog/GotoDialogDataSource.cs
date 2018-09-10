using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Basics.GotoDialog
{
    public class GotoDialogDataSource : INotifyPropertyChanged
    {
        private int _minimumLine;
        private int _maximumLine;
        private int _currentLine;

        public int MinimumLine
        {
            get => _minimumLine;
            set
            {
                if (value == _minimumLine)
                    return;
                _minimumLine = value;
                OnPropertyChanged();
            }
        }

        public int MaximumLine
        {
            get => _maximumLine;
            set
            {
                if (value == _maximumLine)
                    return;
                _maximumLine = value;
                OnPropertyChanged();
            }
        }

        public int CurrentLine
        {
            get => _currentLine;
            set
            {
                if (value == _currentLine)
                    return;
                _currentLine = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
