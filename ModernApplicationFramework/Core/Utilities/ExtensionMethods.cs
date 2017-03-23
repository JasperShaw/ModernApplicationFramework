using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsConnectedToPresentationSource(this DependencyObject obj)
        {
            return PresentationSource.FromDependencyObject(obj) != null;
        }

        public static void RaiseEvent(this EventHandler eventHandler, object source)
        {
            RaiseEvent(eventHandler, source, EventArgs.Empty);
        }

        public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
        {
            eventHandler?.Invoke(source, args);
        }

        public static bool AcquireWin32Focus(this DependencyObject obj, out IntPtr previousFocus)
        {
            HwndSource hwndSource = PresentationSource.FromDependencyObject(obj) as HwndSource;
            if (hwndSource != null)
            {
                previousFocus = User32.GetFocus();
                if (previousFocus != hwndSource.Handle)
                {
                    User32.SetFocus(hwndSource.Handle);
                    return true;
                }
            }
            previousFocus = IntPtr.Zero;
            return false;
        }

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
                if (firstError == null)
                {
                    break;
                }
                var currentException = firstError.Exception;

                if (currentException == null)
                {
                    break;
                }

                var composablePartException = currentException as ComposablePartException;

                if (composablePartException != null
                    && composablePartException.InnerException != null)
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

            return exception; // Fuck it, couldn't find the real deal. Throw the original.
        }
    }
}