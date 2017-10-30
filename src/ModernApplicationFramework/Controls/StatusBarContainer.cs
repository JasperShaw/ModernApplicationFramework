using System;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Primitives;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:ModernApplicationFramework.Controls.ElementContainer" /> for hosting a status bar that inherits from <see cref="T:ModernApplicationFramework.Interfaces.Controls.IStatusBar" />
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.ElementContainer" />
    public class StatusBarContainer : ElementContainer
    {
        protected override string DispatcherGroup => "StatusBar";

        protected override int StackSize => 262144;

        protected override UIElement CreateRootUiElement()
        {
            try
            {
                if (IoC.Get<IStatusBar>() is UIElement statusBar)
                    return statusBar;
            }
            catch (Exception)
            {
                return new BasicStatusBar();
            }
            return new BasicStatusBar();
        }

        protected override bool ShouldForwardPropertyChange(DependencyPropertyChangedEventArgs e)
        {
            return e.Property == DataContextProperty && base.ShouldForwardPropertyChange(e);
        }
    }
}