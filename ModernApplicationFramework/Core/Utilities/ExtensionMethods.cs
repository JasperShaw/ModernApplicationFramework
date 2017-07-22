using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities.Imaging;
using Color = System.Windows.Media.Color;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class ExtensionMethods
    {
        
        public static bool Contains(this IEnumerable collection, object item)
        {
            return collection.Cast<object>().Any(o => o == item);
        }


        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T v in collection)
                action(v);
        }

        public static TV GetValueOrDefault<TV>(this WeakReference wr)
        {
            if (wr == null || !wr.IsAlive)
                return default(TV);
            return (TV)wr.Target;
        }


        public static int IndexOf<T>(this T[] array, T value) where T : class
        {
            for (var i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return i;

            return -1;
        }

        public static System.Exception UnwrapCompositionException(this System.Exception exception)
        {
            var compositionException = exception as CompositionException;
            if (compositionException == null)
            {
                return exception;
            }

            var unwrapped = compositionException;
            while (unwrapped != null)
            {
                var firstError = unwrapped.Errors.FirstOrDefault();
                var currentException = firstError?.Exception;

                if (currentException == null)
                {
                    break;
                }

                var composablePartException = currentException as ComposablePartException;

                if (composablePartException?.InnerException != null)
                {
                    var innerCompositionException = composablePartException.InnerException as CompositionException;
                    if (innerCompositionException == null)
                    {
                        return currentException.InnerException ?? exception;
                    }
                    currentException = innerCompositionException;
                }

                unwrapped = currentException as CompositionException;
            }

            return exception;
        }

        public static void SetThemedIcon(this IThemableIconContainer element)
        {
            element.SetThemedIcon(element.IsEnabled);
        }

        public static void SetThemedIcon(this IThemableIconContainer element, bool isEnabled)
        {
            var vb = element.IconSource as Viewbox;
            if (vb == null)
                return;
            var i = ImageUtilities.IconImageFromFrameworkElement(vb);
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.Linear);

            var b = ImageUtilities.BitmapFromBitmapSource((BitmapSource)i.Source);

            var backgroundColor = ImageThemingUtilities.GetImageBackgroundColor(element as DependencyObject);


            BitmapSource bitmapSource;
            if (isEnabled)
            {
                var bitmap = ImageThemingUtilities.GetThemedBitmap(b, backgroundColor.ToRgba());
                bitmapSource = ImageConverter.BitmapSourceFromBitmap(bitmap);
            }
            else
            {
                var bitmaptSourceOrg = ImageConverter.BitmapSourceFromBitmap(b);
                bitmapSource = ImageThemingUtilities.CreateThemedBitmapSource(bitmaptSourceOrg, backgroundColor, false, Color.FromArgb(64, 255, 255, 255), SystemParameters.HighContrast);
            }

            i.Source = bitmapSource;
            element.Icon = i;
        }

        public static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            int i = 0;
            while (i < list.Count && comparer.Compare(list[i], item) < 0)
                i++;
            list.Insert(i, item);
        }
    }
}