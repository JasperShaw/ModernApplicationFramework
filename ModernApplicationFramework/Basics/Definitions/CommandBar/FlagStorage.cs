using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class FlagStorage : INotifyPropertyChanged
    {
        private bool _pict;
        private bool _pictAndText;
        private bool _textOnly;
        private CommandBarFlags _allFlags;


        public event PropertyChangedEventHandler PropertyChanged;

        public CommandBarFlags AllFlags
        {
            get => _allFlags;
            set
            {
                if (value == _allFlags) return;
                _allFlags = value;
                OnPropertyChanged();
            }
        }

        public bool Pict
        {
            get => _pict;
            set
            {
                if (value == _pict) return;
                _pict = value;
                SetFlag(CommandBarFlags.CommandFlagPict, value);
                OnPropertyChanged();
            }
        }

        public bool TextOnly
        {
            get => _textOnly;
            set
            {
                if (value == _textOnly) return;
                _textOnly = value;
                SetFlag(CommandBarFlags.CommandFlagText, value);
                OnPropertyChanged();
            }
        }

        public bool PictAndText
        {
            get => _pictAndText;
            set
            {
                if (value == _pictAndText) return;
                _pictAndText = value;
                SetFlag(CommandBarFlags.CommandFlagPictAndText, value);
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetFlag(CommandBarFlags flag, bool value)
        {
            var allFlags = AllFlags;
            var vscommandflags = !value ? allFlags & ~flag : allFlags | flag;
            AllFlags = vscommandflags;
        }
    }
}