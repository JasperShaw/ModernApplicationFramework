/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Docking.Controls
{
    public class MenuItemEx : ContextMenuItem
    {
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof (DataTemplate), typeof (MenuItemEx),
                new FrameworkPropertyMetadata(null,
                    OnIconTemplateChanged));

        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector", typeof (DataTemplateSelector), typeof (MenuItemEx),
                new FrameworkPropertyMetadata(null,
                    OnIconTemplateSelectorChanged));

        private bool _reentrantFlag;

        static MenuItemEx()
        {
            IconProperty.OverrideMetadata(typeof (MenuItemEx), new FrameworkPropertyMetadata(OnIconPropertyChanged));
        }

        public DataTemplate IconTemplate
        {
            get { return (DataTemplate) GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the IconTemplateSelector property.  This dependency property 
        /// indicates the DataTemplateSelector for the Icon.
        /// </summary>
        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        protected virtual void OnIconTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateIcon();
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the IconTemplateSelector property.
        /// </summary>
        protected virtual void OnIconTemplateSelectorChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateIcon();
        }

        private static void OnIconPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((MenuItemEx) sender).UpdateIcon();
            }
        }

        private static void OnIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItemEx) d).OnIconTemplateChanged(e);
        }

        /// <summary>
        /// Handles changes to the IconTemplateSelector property.
        /// </summary>
        private static void OnIconTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItemEx) d).OnIconTemplateSelectorChanged(e);
        }

        private void UpdateIcon()
        {
            if (_reentrantFlag)
                return;
            _reentrantFlag = true;
            if (IconTemplateSelector != null)
            {
                var dataTemplateToUse = IconTemplateSelector.SelectTemplate(Icon, this);
                if (dataTemplateToUse != null)
                    Icon = dataTemplateToUse.LoadContent();
            }
            else if (IconTemplate != null)
                Icon = IconTemplate.LoadContent();
            _reentrantFlag = false;
        }
    }
}