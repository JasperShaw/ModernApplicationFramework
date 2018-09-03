using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class TabItemTextControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(MediaTypeNames.Text),
            typeof(string), typeof(TabItemTextControl),
            new PropertyMetadata(string.Empty, OnTextChanged));

        private readonly TextBlock _contentText;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string DisplayedText => _contentText.Text;

        public TabItemTextControl()
        {
            _contentText = new TextBlock();
            Content = _contentText;
        }

        private void UpdateTextContent()
        {
            var availableSize = new Size(double.PositiveInfinity, DesiredSize.Height);
            _contentText.Text = Text;
            _contentText.Measure(availableSize);
            if (_contentText.DesiredSize.Width <= MaxWidth)
                return;
            SplitTextToFit(availableSize);
        }

        private void SplitTextToFit(Size availableSize)
        {
            var rightTrim = Text.Length / 2;
            var leftTrim = Text.Length / 2;
            var num1 = 1;
            var num2 = 1;
            var flag1 = false;
            var flag2 = false;
            var flag3 = true;
            do
            {
                _contentText.Text = InsertEllipsis(Text, leftTrim, rightTrim);
                _contentText.Measure(availableSize);
                if (_contentText.DesiredSize.Width <= MaxWidth)
                {
                    flag1 = true;
                    flag2 = true;
                }
                else if (flag3)
                {
                    flag3 = false;
                    --leftTrim;
                    if (leftTrim < num2)
                    {
                        flag1 = true;
                        leftTrim = num2;
                    }
                }
                else
                {
                    flag3 = true;
                    --rightTrim;
                    if (rightTrim < num1)
                    {
                        flag2 = true;
                        rightTrim = num1;
                    }
                }
            }
            while (!(flag1 & flag2));
        }

        private static void OnTextChanged(DependencyObject depObject, DependencyPropertyChangedEventArgs e)
        {
            (depObject as TabItemTextControl)?.UpdateTextContent();
        }

        private static string InsertEllipsis(string tabText, int leftTrim, int rightTrim)
        {
            return tabText.Substring(0, leftTrim).TrimEnd(' ') + "…" +
                   tabText.Substring(tabText.Length - rightTrim).TrimStart(' ');
        }
    }
}
