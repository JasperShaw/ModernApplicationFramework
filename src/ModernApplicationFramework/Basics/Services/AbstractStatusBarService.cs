using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Caliburn.Micro;
using JetBrains.Annotations;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract implementation of an <see cref="IStatusBarDataModelService" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Services.IStatusBarDataModelService" />
    public abstract class AbstractStatusBarService : IStatusBarDataModelService
    {
        /// <summary>
        /// A selection of default background colors
        /// </summary>
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
        /// <inheritdoc />
        /// <summary>
        /// Indicates whether the status bar is visible
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// Returns the main status text
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// The maximal units of the ProgressBar
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// The current Value of the ProgressBar
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// Indicates whether the ProgressBar is visible
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// The Background color the StatusBar
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// The Foreground color the StatusBar
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// Freezes the output.
        /// </summary>
        /// <param name="fFreeze">The f freeze.</param>
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

        /// <inheritdoc />
        /// <summary>
        /// Gets the freeze count.
        /// </summary>
        /// <returns></returns>
        public int GetFreezeCount()
        {
            return _freezeCount;
        }

        /// <inheritdoc />
        /// <summary>
        /// Determines whether this instance is frozen.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is frozen; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFrozen()
        {
            return _freezeCount > 0;
        }


        /// <inheritdoc />
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (_syncObj)
            {
                ResetFields();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Progresses the specified in progress.
        /// </summary>
        /// <param name="inProgress">if set to <c>true</c> [in progress].</param>
        /// <param name="label">The label.</param>
        /// <param name="complete">The complete.</param>
        /// <param name="total">The total.</param>
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

        /// <inheritdoc />
        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetText(string text)
        {
            SetText(0, text);
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="text">The text.</param>
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

        /// <inheritdoc />
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return GetTextInternal(0);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetText(int index)
        {
            return GetTextInternal(index);
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the ready text.
        /// </summary>
        public void SetReadyText()
        {
            var ready = MainWindowResources.StatusBarText_Ready;
            lock (_syncObj)
            {
                Text = ready;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="dwVisibility">The visibility.</param>
        /// <returns></returns>
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

        /// <inheritdoc />
        /// <summary>
        /// Gets the visibility.
        /// </summary>
        /// <returns></returns>
        public bool GetVisibility()
        {
            return IsVisible;
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the color of the background.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetBackgroundColor(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            Background = brush;
            Foreground = GetForegroundByBackground(color);
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the color of the background.
        /// </summary>
        /// <param name="colorType">Type of the color.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">color - null</exception>
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

        /// <summary>
        /// Sets a text field based on the index.
        /// </summary>
        /// <param name="index">The index of the text field that should be filled</param>
        /// <param name="text">The text</param>
        protected abstract void SetTextInternal(int index, string text);

        /// <summary>
        /// Gets the text of a text field by index.
        /// </summary>
        /// <param name="index">The index of the text field</param>
        /// <returns>The text</returns>
        protected abstract string GetTextInternal(int index);

        private bool GetUserVisibilityPreference()
        {
            return _generalOptions.ShowStatusBar;
        }

        /// <summary>
        /// Resets the status bar.
        /// </summary>
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}