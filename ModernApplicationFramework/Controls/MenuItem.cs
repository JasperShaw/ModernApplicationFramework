using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Test;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Controls
{
    public class MenuItem : System.Windows.Controls.MenuItem
    {

        private readonly object _iconSource;

        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
        }

        public MenuItem(DefinitionBase definition)
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
            DataContext = definition;
            if (string.IsNullOrEmpty(definition.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = definition.IconSource };
            _iconSource = myResourceDictionary[definition.IconId];
        }

        private void ThemeManager_OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            SetIcon();
        }

        public MenuItem()
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
        }


        public static MenuItem CreateItem(MenuDefinition definition)
        {
            return new MenuItem { Header = definition.DisplayName };
        }

        public void SetIcon()
        {
            if (_iconSource == null)
                return;
            var vb = _iconSource as Viewbox;
            var i = ImageUtilities.IconImageFromFrameworkElement(vb);
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Fant);

            var b = ImageUtilities.BitmapFromBitmapSource((BitmapSource)i.Source);
            var bi = ImageThemingUtilities.GetThemedBitmap(b, ImageThemingUtilities.GetImageBackgroundColor(this).ToRgba());
            var bs = ImageConverter.BitmapSourceFromBitmap(bi);
            i.Source = bs;

            //var viewBox = new Viewbox();
            //viewBox.Height = 16;
            //viewBox.Width = 16;
            //viewBox.Child = i;

            Icon = i;
        }

        /// <summary>
        ///     Creates a MenuItem from CommandDefinition. If the Attached Command is Type GestureCommand the Item will bind the
        ///     Gesture text
        /// </summary>
        /// <param name="definition"></param>
        /// <returns>MenuItem</returns>
        public static MenuItem CreateItemFromDefinition(DefinitionBase definition)
        {
            var menuItem = new MenuItem(definition)
            {
                Header = definition.Text,
            };
            if (!(definition is CommandDefinition commandDefinition))
                return menuItem;
            var myBindingC = new Binding
            {
                Source = definition,
                Path = new PropertyPath(nameof(commandDefinition.Command)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, CommandProperty, myBindingC);

            var c = commandDefinition.Command as GestureCommandWrapper;
            if (c == null)
                return menuItem;

            var myBinding = new Binding
            {
                Source = c,
                Path = new PropertyPath(nameof(c.GestureText)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(menuItem, InputGestureTextProperty,
                myBinding);
            if (commandDefinition.IsChecked)
                menuItem.IsChecked = true;
            menuItem.SetIcon();
            return menuItem;
        }
    } 
}