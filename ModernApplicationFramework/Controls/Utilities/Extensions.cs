using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Utilities
{
	internal static class Extensions
	{
		internal static  bool IsValid(this CornerRadius cornerRadius, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
		{
			if (!allowNegative)
			{
				if (cornerRadius.TopLeft < 0d || cornerRadius.TopRight < 0d || cornerRadius.BottomLeft < 0d || cornerRadius.BottomRight < 0d)
				{
					return false;
				}
			}

			if (!allowNaN)
			{
				if (IsNaN(cornerRadius.TopLeft) || IsNaN(cornerRadius.TopRight) || IsNaN(cornerRadius.BottomLeft) || IsNaN(cornerRadius.BottomRight))
				{
					return false;
				}
			}

			if (!allowPositiveInfinity)
			{
				if (double.IsPositiveInfinity(cornerRadius.TopLeft) || double.IsPositiveInfinity(cornerRadius.TopRight) || double.IsPositiveInfinity(cornerRadius.BottomLeft) || double.IsPositiveInfinity(cornerRadius.BottomRight))
				{
					return false;
				}
			}

			if (!allowNegativeInfinity)
			{
				if (double.IsNegativeInfinity(cornerRadius.TopLeft) || double.IsNegativeInfinity(cornerRadius.TopRight) || double.IsNegativeInfinity(cornerRadius.BottomLeft) || double.IsNegativeInfinity(cornerRadius.BottomRight))
				{
					return false;
				}
			}

			return true;
		}


		[StructLayout(LayoutKind.Explicit)]
		private struct NanUnion
		{
			[FieldOffset(0)] internal double DoubleValue;
			[FieldOffset(0)] internal readonly ulong UintValue;
		}
    
		public static bool IsNaN(double value)
		{
			var t = new NanUnion {DoubleValue = value};
			var exp = t.UintValue & 0xfff0000000000000;
			var man = t.UintValue & 0x000fffffffffffff;
			return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
		}

	    public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : class
	    {
	        if (obj == null)
	            return Enumerable.Empty<T>();
	        var descendants = new List<T>();
	        obj.TraverseVisualTree<T>(child => descendants.Add(child));
	        return descendants;
	    }

	    public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class
	    {
	        if (obj == null)
	            return;
	        for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
	        {
	            DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
	            if (child != null)
	            {
	                T obj1 = child as T;
	                child.TraverseVisualTreeReverse(action);
	                if (obj1 != null)
	                    action(obj1);
	            }
	        }
	    }

	    public static void TraverseVisualTreeReverse<T>(this DependencyObject obj, Action<T> action) where T : class
	    {
	        if (obj == null)
	            return;
	        for (int childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex)
	        {
	            DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
	            if (child != null)
	            {
	                T obj1 = child as T;
	                child.TraverseVisualTreeReverse(action);
	                if (obj1 != null)
	                    action(obj1);
	            }
	        }
	    }
    }
}
