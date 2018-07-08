using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities.Imaging;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using ImageConverter = ModernApplicationFramework.Utilities.Imaging.ImageConverter;

namespace ModernApplicationFramework.Core.Utilities
{
    /// <summary>
    /// Set of extension methods
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Determines whether an <see langword="object"/> is inside an<see cref="IEnumerable"/>
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <see langword="true"/>if the object is inside the collection; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Contains(this IEnumerable collection, object item)
        {
            return collection.Cast<object>().Any(o => o == item);
        }


        /// <summary>
        /// Performs a <see cref="Action{T}"/> for each item of an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">the generic type of the collection</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var v in collection)
                action(v);
        }

        internal static TV GetValueOrDefault<TV>(this WeakReference wr)
        {
            if (wr == null || !wr.IsAlive)
                return default;
            return (TV)wr.Target;
        }


        /// <summary>
        /// Gets the index of an item inside an array of a type T
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns the index of the item. Returns -1 if the item was not inside the array</returns>
        public static int IndexOf<T>(this T[] array, T value) where T : class
        {
            for (var i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return i;

            return -1;
        }

        /// <summary>
        /// Unwraps a composition exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The unwrapped exception</returns>
        public static System.Exception UnwrapCompositionException(this System.Exception exception)
        {
            if (!(exception is CompositionException compositionException))
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
                    if (!(composablePartException.InnerException is CompositionException innerCompositionException))
                    {
                        return currentException.InnerException ?? exception;
                    }
                    currentException = innerCompositionException;
                }

                unwrapped = currentException as CompositionException;
            }

            return exception;
        }

        /// <summary>
        /// Sets a themed icon for an <see cref="IThemableIconContainer"/>
        /// </summary>
        /// <param name="element">The element.</param>
        public static void SetThemedIcon(this IThemableIconContainer element)
        {
            element.SetThemedIcon(element.IsEnabled);
        }


        public static void SetThemedIcon(this IThemableIconContainer element, Color backgroundColor, bool isEnabled = true)
        {

            var vb = element.IconSource as Viewbox;

            Image image;
            if (vb == null)
            {
                if (element.Icon == null)
                    return;
                if (element.Icon is Image elementIcon)
                    image = elementIcon;
                else
                    return;
            }
            else
                image = ImageUtilities.IconImageFromFrameworkElement(vb);
            
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.Linear);

            Bitmap b;
            if (image.DataContext is IHasIconSource layoutItem)
            {
                var bi = layoutItem.IconSource as BitmapSource;
                b = ImageUtilities.BitmapFromBitmapSource(bi);
            }
            else
                b = ImageUtilities.BitmapFromBitmapSource((BitmapSource) image.Source);

            BitmapSource bitmapSource;
            if (isEnabled)
            {
                var bitmap = ImageThemingUtilities.GetThemedBitmap(b, backgroundColor.ToRgba());
                bitmapSource = ImageConverter.BitmapSourceFromBitmap(bitmap);
            }
            else
            {
                var bitmapSourceOrg = ImageConverter.BitmapSourceFromBitmap(b);
                bitmapSource = ImageThemingUtilities.CreateThemedBitmapSource(bitmapSourceOrg, backgroundColor, false, Color.FromArgb(64, 255, 255, 255), SystemParameters.HighContrast);
            }

            image.Source = bitmapSource;
            element.Icon = image;
        }


        /// <summary>
        ///  Sets a themed icon for an <see cref="IThemableIconContainer"/>. Supports gray scale if the element is not enabled
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="isEnabled">if set to <see langword="true"/> no gray scale will be applied</param>
        public static void SetThemedIcon(this IThemableIconContainer element, bool isEnabled)
        {
            var backgroundColor = ImageThemingUtilities.GetImageBackgroundColor(element as DependencyObject);
            SetThemedIcon(element, backgroundColor, isEnabled);
        }

        internal static void AddSorted<T>(this IList<T> list, T item, IComparer<T> comparer = null)
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