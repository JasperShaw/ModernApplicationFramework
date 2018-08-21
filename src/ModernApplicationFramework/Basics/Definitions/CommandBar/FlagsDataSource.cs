using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A storage to collect flags
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public class FlagsDataSource : INotifyPropertyChanged
    {
        private bool _pict;
        private bool _pictAndText;
        private bool _textOnly;
        private uint _allFlags;
        private bool _stretchHorizontally;
        private bool _textIsAnchor;
        private bool _fixMenuController;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// All flags currently saved
        /// </summary>
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

        /// <summary>
        /// Option to show a picture 
        /// </summary>
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

        /// <summary>
        /// Option to show the text only 
        /// </summary>
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

        /// <summary>
        /// Option to text and picture
        /// </summary>
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

        /// <summary>
        /// Option to stretch horizontally
        /// </summary>
        public bool StretchHorizontally
        {
            get => _stretchHorizontally;
            set
            {
                if (value == _stretchHorizontally) return;
                _stretchHorizontally = value;
                SetFlag(CommandBarFlags.CommandStretchHorizontally, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Option whether text is anchored
        /// </summary>
        public bool TextIsAnchor
        {
            get => _textIsAnchor;
            set
            {
                if (value == _textIsAnchor) return;
                _textIsAnchor = value;
                SetFlag(CommandBarFlags.CommandFlagTextIsAnchor, value);
                OnPropertyChanged();
            }
        }

        public bool FixMenuController
        {
            get => _fixMenuController;
            set
            {
                if (value == _fixMenuController) return;
                _fixMenuController = value;
                SetFlag(CommandBarFlags.CommandFlagTextIsAnchor, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enables the style flags.
        /// </summary>
        /// <param name="flagToEnable">The flag to enable.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                case CommandBarFlags.CommandFlagTextIsAnchor:
                    TextIsAnchor = true;
                    break;
                case CommandBarFlags.CommandStretchHorizontally:
                    StretchHorizontally = true;
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