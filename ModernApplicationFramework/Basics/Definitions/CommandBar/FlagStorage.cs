using System;
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
        private uint _allFlags;


        public event PropertyChangedEventHandler PropertyChanged;

        public uint AllFlags
        {
            get => _allFlags;
            set
            {
                if (value == _allFlags)
                    return;
                _allFlags = value;
                OnPropertyChanged();
            }
        }

        public bool Pict
        {
            get => _pict;
            set
            {
                if (value == _pict)
                    return;
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
                if (value == _textOnly)
                    return;
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
                if (value == _pictAndText)
                    return;
                _pictAndText = value;
                SetFlag(CommandBarFlags.CommandFlagPictAndText, value);
                OnPropertyChanged();
            }
        }

        public void EnableStyleFlags(CommandBarFlags flagToEnable)
        {
            switch (flagToEnable)
            {
                case CommandBarFlags.CommandFlagNone:
                    Pict = false;
                    PictAndText = false;
                    TextOnly = false;
                    break;
                case CommandBarFlags.CommandFlagPict:
                    Pict = true;
                    PictAndText = false;
                    TextOnly = false;
                    break;
                case CommandBarFlags.CommandFlagText:
                    Pict = false;
                    PictAndText = false;
                    TextOnly = true;
                    break;
                case CommandBarFlags.CommandFlagPictAndText:
                    Pict = true;
                    PictAndText = true;
                    TextOnly = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            var commandflags = !value ? allFlags & (uint) ~flag : allFlags | (uint) flag;
            AllFlags = commandflags;
        }
    }
}