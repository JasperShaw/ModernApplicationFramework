using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    internal class OverviewScrollBarViewModel : INotifyPropertyChanged
    {
        private Brush _scrollBarArrowBackground;
        private Brush _scrollBarArrowGlyph;
        private Brush _scrollBarArrowMouseOverBackground;
        private Brush _scrollBarArrowMouseOverGlyph;
        private Brush _scrollBarArrowPressedBackground;
        private Brush _scrollBarArrowPressedGlyph;

        public event PropertyChangedEventHandler PropertyChanged;

        public OverviewScrollBarViewModel(Action upAction, Action downAction)
        {
            var action1 = upAction;
            if (action1 == null)
                throw new ArgumentNullException(nameof(upAction));
            UpCommand = new ActionCommand(action1);
            var action2 = downAction;
            if (action2 == null)
                throw new ArgumentNullException(nameof(downAction));
            DownCommand = new ActionCommand(action2);
        }

        public ICommand UpCommand { get; }

        public ICommand DownCommand { get; }

        public Brush ScrollBarArrowBackground
        {
            get => _scrollBarArrowBackground;
            set
            {
                if (ScrollBarArrowBackground == value)
                    return;
                _scrollBarArrowBackground = value;
                RaisePropertyChanged(nameof(ScrollBarArrowBackground));
            }
        }

        public Brush ScrollBarArrowGlyph
        {
            get => _scrollBarArrowGlyph;
            set
            {
                if (ScrollBarArrowGlyph == value)
                    return;
                _scrollBarArrowGlyph = value;
                RaisePropertyChanged(nameof(ScrollBarArrowGlyph));
            }
        }

        public Brush ScrollBarArrowMouseOverBackground
        {
            get => _scrollBarArrowMouseOverBackground;
            set
            {
                if (ScrollBarArrowMouseOverBackground == value)
                    return;
                _scrollBarArrowMouseOverBackground = value;
                RaisePropertyChanged(nameof(ScrollBarArrowMouseOverBackground));
            }
        }

        public Brush ScrollBarArrowMouseOverGlyph
        {
            get => _scrollBarArrowMouseOverGlyph;
            set
            {
                if (ScrollBarArrowMouseOverGlyph == value)
                    return;
                _scrollBarArrowMouseOverGlyph = value;
                RaisePropertyChanged(nameof(ScrollBarArrowMouseOverGlyph));
            }
        }

        public Brush ScrollBarArrowPressedBackground
        {
            get => _scrollBarArrowPressedBackground;
            set
            {
                if (ScrollBarArrowPressedBackground == value)
                    return;
                _scrollBarArrowPressedBackground = value;
                RaisePropertyChanged(nameof(ScrollBarArrowPressedBackground));
            }
        }

        public Brush ScrollBarArrowPressedGlyph
        {
            get => _scrollBarArrowPressedGlyph;
            set
            {
                if (_scrollBarArrowPressedGlyph == value)
                    return;
                _scrollBarArrowPressedGlyph = value;
                RaisePropertyChanged(nameof(ScrollBarArrowPressedGlyph));
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
