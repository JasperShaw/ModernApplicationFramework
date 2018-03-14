using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.MostRecentlyUsedManager
{
    public abstract class MruItem : IHasTextProperty
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

        protected abstract string PersistenceData { get; }

        public abstract bool Matches(string stringValue);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}