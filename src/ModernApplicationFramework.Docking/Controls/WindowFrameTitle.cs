using System.Windows;

namespace ModernApplicationFramework.Docking.Controls
{
    public class WindowFrameTitle : DependencyObject
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(WindowFrameTitle), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public WindowFrameTitle(string title)
        {
            Title = title;
        }
    }
}
