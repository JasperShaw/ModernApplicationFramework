using System.Windows;
using Caliburn.Micro;

namespace ModernApplicationFramework.Controls
{
    public class StatusBar : System.Windows.Controls.Primitives.StatusBar
    {
        public static readonly DependencyProperty ModeTextProperty = DependencyProperty.Register(
            "ModeText", typeof(string), typeof(StatusBar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
            "StatusText", typeof(string), typeof(StatusBar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(int), typeof(StatusBar), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty InformationTextAProperty = DependencyProperty.Register(
            "InformationTextA", typeof(string), typeof(StatusBar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty InformationTextBProperty = DependencyProperty.Register(
            "InformationTextB", typeof(string), typeof(StatusBar), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty InformationTextCProperty = DependencyProperty.Register(
            "InformationTextC", typeof(string), typeof(StatusBar), new PropertyMetadata(default(string)));

        private int _lastMode;

        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }

        public string InformationTextA
        {
            get { return (string) GetValue(InformationTextAProperty); }
            set { SetValue(InformationTextAProperty, value); }
        }

        public string InformationTextB
        {
            get { return (string) GetValue(InformationTextBProperty); }
            set { SetValue(InformationTextBProperty, value); }
        }

        public string InformationTextC
        {
            get { return (string) GetValue(InformationTextCProperty); }
            set { SetValue(InformationTextCProperty, value); }
        }

        public int Mode
        {
            get { return (int) GetValue(ModeProperty); }
            set
            {
                _lastMode = Mode;
                SetValue(ModeProperty, value);
            }
        }

        public string ModeText
        {
            get { return (string) GetValue(ModeTextProperty); }
            set { SetValue(ModeTextProperty, value); }
        }

        public string StatusText
        {
            get { return (string) GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }

        public void RestoreMode()
        {
            Execute.OnUIThread(() => SetValue(ModeProperty, _lastMode));
        }
    }
}