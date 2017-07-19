using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.Services
{
    public abstract class AbstractStatusBarService : IStatusBarDataModelService
    {
        public enum DefaultColors
        {
            Blue,
            DarkBlue,
            Organe,
            Purple
        }

        private static IStatusBarDataModelService _instance;
        private readonly EnvironmentGeneralOptions _generalOptions;

        private readonly object _syncObj = new object();
        private Brush _background;
        private Brush _foreground;
        private int _freezeCount;
        private bool _isProgressBarActive;
        private bool _isVisible;
        private uint _progressBarMax;
        private uint _progressBarValue;
        private string _text;
        private string _textBeforeProgressBar;

        protected AbstractStatusBarService()
        {
            try
            {
                _generalOptions = IoC.Get<EnvironmentGeneralOptions>();
                IsVisible = GetUserVisibilityPreference();
            }
            catch
            {
                //Ignored
            }
        }

        public static IStatusBarDataModelService Instance => _instance ??
                                                             (_instance = IoC.Get<IStatusBarDataModelService>());

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public uint ProgressBarMax
        {
            get => _progressBarMax;
            set
            {
                if (value == _progressBarMax) return;
                _progressBarMax = value;
                OnPropertyChanged();
            }
        }

        public uint ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                if (value == _progressBarValue) return;
                _progressBarValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsProgressBarActive
        {
            get => _isProgressBarActive;
            set
            {
                if (value == _isProgressBarActive) return;
                _isProgressBarActive = value;
                OnPropertyChanged();
            }
        }

        public Brush Background
        {
            get => _background;
            set
            {
                if (Equals(value, _background)) return;
                _background = value;
                OnPropertyChanged();
            }
        }

        public Brush Foreground
        {
            get => _foreground;
            set
            {
                if (Equals(value, _foreground)) return;
                _foreground = value;
                OnPropertyChanged();
            }
        }

        public void FreezeOutput(int fFreeze)
        {
            lock (_syncObj)
            {
                if (fFreeze != 0)
                    _freezeCount = _freezeCount++;
                else if (_freezeCount != 0)
                    _freezeCount = _freezeCount--;
            }
        }

        public int GetFreezeCount()
        {
            return _freezeCount;
        }

        public bool IsFrozen()
        {
            return _freezeCount > 0;
        }


        public void Clear()
        {
            lock (_syncObj)
            {
                ResetFields();
            }
        }

        public void Progress(bool inProgress, string label, uint complete, uint total)
        {
            lock (_syncObj)
            {
                if (inProgress)
                {
                    if (_textBeforeProgressBar == null)
                        _textBeforeProgressBar = Text;
                    Text = label ?? string.Empty;
                    ProgressBarMax = total;
                }
                else
                {
                    complete = 0;
                    if (_textBeforeProgressBar != null)
                    {
                        Text = _textBeforeProgressBar;
                        _textBeforeProgressBar = null;
                    }
                }
                ProgressBarValue = complete;
                IsProgressBarActive = complete > 0;
            }
        }

        public void SetText(string text)
        {
            SetText(0, text);
        }

        public void SetText(int index, string text)
        {
            if (_freezeCount > 0)
                return;
            lock (_syncObj)
            {
                if (text != null)
                {
                    var stringBuilder = new StringBuilder(text.Length);
                    foreach (var ch in text)
                        stringBuilder.Append(ch < 32 ? ' ' : ch);
                    SetTextInternal(index, stringBuilder.ToString());
                }
                else
                {
                    SetTextInternal(index, string.Empty);
                }
            }
        }

        public string GetText()
        {
            return GetTextInternal(0);
        }

        public string GetText(int index)
        {
            return GetTextInternal(index);
        }

        public void SetReadyText()
        {
            var ready = MainWindowResources.StatusBarText_Ready;
            lock (_syncObj)
            {
                if (string.IsNullOrEmpty(Text))
                    Text = ready;
            }
        }

        public int SetVisibility(uint dwVisibility)
        {
            switch (dwVisibility)
            {
                case 0:
                    IsVisible = false;
                    break;
                case 1:
                    IsVisible = true;
                    break;
                default:
                    return -1;
            }
            return 0;
        }

        public bool GetVisibility()
        {
            return IsVisible;
        }

        public void SetBackgroundColor(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            Background = brush;
            Foreground = GetForegroundByBackground(color);
        }

        public void SetBackgroundColor(DefaultColors colorType)
        {
            Color color;
            switch (colorType)
            {
                case DefaultColors.Blue:
                    color = HexStringToBrush("#FF007ACC");
                    break;
                case DefaultColors.DarkBlue:
                    color = HexStringToBrush("#FF0E639C");
                    break;
                case DefaultColors.Organe:
                    color = HexStringToBrush("#FFCA5100");
                    break;
                case DefaultColors.Purple:
                    color = HexStringToBrush("#FF68217A");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), colorType, null);
            }
            SetBackgroundColor(color);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected abstract void SetTextInternal(int index, string text);

        protected abstract string GetTextInternal(int index);

        private bool GetUserVisibilityPreference()
        {
            return _generalOptions.ShowStatusBar;
        }

        protected virtual void ResetFields()
        {
            Text = string.Empty;
        }

        private Color HexStringToBrush(string hex)
        {
            if (!(ColorConverter.ConvertFromString(hex) is Color color))
                return Colors.Transparent;
            return color;
        }

        private static Brush GetForegroundByBackground(Color color)
        {
            var brightness = (int) Math.Sqrt(
                color.R * color.R * .299 +
                color.G * color.G * .587 +
                color.B * color.B * .114);
            return brightness > 130 ? Brushes.Black : Brushes.White;
        }

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}