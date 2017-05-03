using System.Runtime.InteropServices;
using System.Windows;

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
	}
}
