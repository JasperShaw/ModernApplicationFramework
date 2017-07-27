using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Settings.Interfaces;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    internal class SettingsPageStackPanel : StackPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var mySize = new Size();
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
                mySize.Width += child.DesiredSize.Width;
                mySize.Height += child.DesiredSize.Height;
            }
            return mySize;
        }

        protected override Size ArrangeOverride(Size availableSize)
        {
            if (InternalChildren.Count != 1)
                return base.ArrangeOverride(availableSize);
            if (!(InternalChildren[0] is ContentPresenter contentPresenter)
                || !(contentPresenter.Content is IStretchSettingsPanelPanel))
                return base.ArrangeOverride(availableSize);

            var widthSum = contentPresenter.DesiredSize.Width;;
            var proportionalWidth = contentPresenter.DesiredSize.Width / widthSum * availableSize.Width;
            contentPresenter.Arrange(
                new Rect(
                    new Point(0.0, 0.0),
                    new Point(0.0 + proportionalWidth, availableSize.Height)));
            return availableSize;
        }
    }
}
