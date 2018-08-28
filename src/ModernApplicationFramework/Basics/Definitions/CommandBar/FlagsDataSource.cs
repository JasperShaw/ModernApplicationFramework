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
        private CommandBarFlags _allFlags;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// All flags currently saved
        /// </summary>
        public CommandBarFlags AllFlags
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
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagPict);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagPict, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Option to show the text only 
        /// </summary>
        public bool TextOnly
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagText);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagText, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Option to text and picture
        /// </summary>
        public bool PictAndText
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagPictAndText);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagPictAndText, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Option to stretch horizontally
        /// </summary>
        public bool StretchHorizontally
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandStretchHorizontally);
            set
            {
                SetFlag(CommandBarFlags.CommandStretchHorizontally, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Option whether text is anchored
        /// </summary>
        public bool TextIsAnchor
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagTextIsAnchor);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagTextIsAnchor, value);
                OnPropertyChanged();
            }
        }

        public bool FixMenuController
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagFixMenuController);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagFixMenuController, value);
                OnPropertyChanged();
            }
        }

        public bool FilterKeys
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFilterKeys);
            set
            {
                SetFlag(CommandBarFlags.CommandFilterKeys, value);
                OnPropertyChanged();
            }
        }

        public bool ComboCommitsOnDrop
        {
            get => _allFlags.HasFlag(CommandBarFlags.CommandFlagComboCommitsOnDrop);
            set
            {
                SetFlag(CommandBarFlags.CommandFlagComboCommitsOnDrop, value);
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
            _allFlags = CommandBarFlags.CommandFlagNone;
            //SetFlag(CommandBarFlags.CommandFlagNone, true, false);
            SetFlag(flagToEnable, true);
        }

        private void NotifyAll()
        {
            OnPropertyChanged(nameof(ComboCommitsOnDrop));
            OnPropertyChanged(nameof(FilterKeys));
            OnPropertyChanged(nameof(Pict));
            OnPropertyChanged(nameof(PictAndText));
            OnPropertyChanged(nameof(TextOnly));
            OnPropertyChanged(nameof(TextIsAnchor));
            OnPropertyChanged(nameof(StretchHorizontally));
            OnPropertyChanged(nameof(FixMenuController));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetFlag(CommandBarFlags flag, bool value, bool notify = true)
        {
            var allFlags = AllFlags;
            var commandflags = !value ? allFlags & ~flag : allFlags | flag;
            AllFlags = commandflags;
            if (notify)
                NotifyAll();
        }
    }
}