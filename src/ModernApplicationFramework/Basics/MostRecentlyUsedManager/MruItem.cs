using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.MostRecentlyUsedManager
{
    public abstract class MruItem : IMruItem
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _pinned;
        private string _text;

        public bool Pinned
        {
            get => _pinned;
            set
            {
                if (value == _pinned)
                    return;
                _pinned = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                    return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public abstract object PersistenceData { get; }

        public abstract bool Matches(object data);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}