using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class ScrollBarThemingUtilities
    {
        private static readonly WeakCollection<FrameworkElement> WeakUnthemedScrollingElements = new WeakCollection<FrameworkElement>();

        static ScrollBarThemingUtilities()
        {
            IoC.Get<IThemeManager>().OnThemeChanged += OnThemeChanged;
            ImageThemingUtilities.ThemeScrollBarsChanged += OnThemeScrollBarsChanged;
            RepublishUnthemedScrollBarStyles();    
        }

        private static void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ImageThemingUtilities.ClearWeakImageCache();
            UpdateUnthemedScrollBarStyles();
        }

        private static void OnThemeScrollBarsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement element))
                return;
            if (element.IsConnectedToPresentationSource())
                UpdateScrollBarStyles(element);
            else
                PresentationSource.AddSourceChangedHandler(element, OnPresentationSourceChanged);
        }

        private static void UpdateScrollBarStyles(FrameworkElement element)
        {
            Validate.IsNotNull(element, nameof(element));
            var themeScrollBars = ImageThemingUtilities.GetThemeScrollBars(element);
            if (themeScrollBars.HasValue)
            {
                SetScrollBarStyles(element, themeScrollBars.Value);
                if (themeScrollBars.Value)
                    WeakUnthemedScrollingElements.Remove(element);
                else
                    WeakUnthemedScrollingElements.Add(element);
            }
            else
            {
                RemoveScrollBarStyles(element);
                WeakUnthemedScrollingElements.Remove(element);
            }
        }

        private static void UpdateUnthemedScrollBarStyles()
        {
            RepublishUnthemedScrollBarStyles();
            ReapplyUnthemedScrollBarStyles();
        }



        private static void RemoveScrollBarStyles(FrameworkElement element)
        {
            Validate.IsNotNull(element, nameof(element));
            element.Resources.Remove(typeof(ScrollBar));
            element.Resources.Remove(typeof(ScrollViewer));
            element.Resources.Remove(GridView.GridViewScrollViewerStyleKey);
        }

        private static void SetScrollBarStyles(FrameworkElement element, bool themeScrollBars)
        {
            Validate.IsNotNull(element, nameof(element));
            element.Resources[typeof(ScrollBar)] = element.TryFindResource(ResourceKeys.GetScrollBarStyleKey(themeScrollBars));
            element.Resources[typeof(ScrollViewer)] = element.TryFindResource(ResourceKeys.GetScrollViewerStyleKey(themeScrollBars));
            element.Resources[GridView.GridViewScrollViewerStyleKey] = element.TryFindResource(ResourceKeys.GetGridViewScrollViewerStyleKey(themeScrollBars));
        }

        private static void RepublishUnthemedScrollBarStyles()
        {
            RepublishApplicationResource(typeof(ScrollBar), ResourceKeys.UnthemedScrollBarStyleKey);
            RepublishApplicationResource(typeof(ScrollViewer), ResourceKeys.UnthemedScrollViewerStyleKey);
            RepublishApplicationResource(GridView.GridViewScrollViewerStyleKey, ResourceKeys.UnthemedGridViewScrollViewerStyleKey);
        }

        private static void ReapplyUnthemedScrollBarStyles()
        {
            foreach (var element in WeakUnthemedScrollingElements.ToList())
                SetScrollBarStyles(element, false);
        }

        private static void RepublishApplicationResource(object existingKey, object newKey)
        {
            if (Application.Current == null)
                return;
            var resource = Application.Current.TryFindResource(existingKey);
            if (resource == null)
                return;
            Application.Current.Resources[newKey] = resource;
        }

        private static void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
                UpdateScrollBarStyles(element);
            PresentationSource.RemoveSourceChangedHandler(element, OnPresentationSourceChanged);

        }
    }
}
