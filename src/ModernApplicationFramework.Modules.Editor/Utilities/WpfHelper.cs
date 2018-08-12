using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Modules.Editor.NativeMethods;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public static class WpfHelper
    {

        [ThreadStatic]
        private static NativeMethods.NativeMethods.ITfThreadMgr _threadMgr;
        [ThreadStatic]
        private static bool _threadMgrFailed;

        public static readonly double DeviceScaleX;
        public static readonly double DeviceScaleY;

        static WpfHelper()
        {
            var dc = User32.GetDC(IntPtr.Zero);
            if (dc != IntPtr.Zero)
            {
                DeviceScaleX = 96.0 / User32.GetDeviceCaps(dc, 88);
                DeviceScaleY = 96.0 / User32.GetDeviceCaps(dc, 90);
                User32.ReleaseDC(IntPtr.Zero, dc);
            }
            else
            {
                DeviceScaleX = 1.0;
                DeviceScaleY = 1.0;
            }
        }

        public static Cursor LoadCursorDpiAware(Stream cursorStream)
        {
            var filePath = string.Empty;
            try
            {
                using (new BinaryReader(cursorStream))
                {
                    using (var randomFileNameStream = GetRandomFileNameStream(Path.GetTempPath(), out filePath))
                        cursorStream.CopyTo(randomFileNameStream);
                }

                var safeCursor = new SafeCursor(User32.LoadImage(IntPtr.Zero, filePath,
                    NativeMethods.NativeMethods.ImageType.ImageCursor, 0, 0,
                    NativeMethods.NativeMethods.ImageFormatRequest.LrLoadfromfile |
                    NativeMethods.NativeMethods.ImageFormatRequest.LrDefaultsize));
                if (safeCursor.IsInvalid)
                    return null;
                return CursorInteropHelper.Create(safeCursor);
            }
            catch
            {
                // ignored
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            return null;
        }

        private static FileStream GetRandomFileNameStream(string fileDirectory, out string filePath)
        {
            var num = 0;
            filePath = string.Empty;
            while (num++ < 2)
            {
                var randomFileName = Path.GetRandomFileName();
                filePath = Path.Combine(fileDirectory, randomFileName + "~");
                if (!File.Exists(filePath))
                {
                    try
                    {
                        return new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            throw new IOException(filePath + " exists");
        }

        public static bool HanjaConversion(IntPtr context, IntPtr keyboardLayout, char selection)
        {
            if (context != IntPtr.Zero)
            {
                var hglobalUni = Marshal.StringToHGlobalUni(new string(selection, 1));
                var num = Imm32.ImmEscapeW(keyboardLayout, context, 4104, hglobalUni);
                Marshal.FreeHGlobal(hglobalUni);
                var zero = IntPtr.Zero;
                if (num != zero)
                    return true;
            }
            return false;
        }

        public static IntPtr GetKeyboardLayout()
        {
            return User32.GetKeyboardLayout(0);
        }

        public static bool TypefacesEqual(Typeface typeface, Typeface other)
        {
            if (typeface == null)
                return other == null;
            return typeface.Equals(other);
        }

        public static IntPtr AttachContext(HwndSource hwndSource, IntPtr imeContext)
        {
            if (hwndSource == null)
                throw new ArgumentNullException(nameof(hwndSource));
            return Imm32.ImmAssociateContext(hwndSource.Handle, imeContext);
        }

        public static IntPtr GetDefaultImeWnd()
        {
            return Imm32.ImmGetDefaultIMEWnd(IntPtr.Zero);
        }

        public static IntPtr GetImmContext(IntPtr hwnd)
        {
            return hwnd != IntPtr.Zero ? Imm32.ImmGetContext(hwnd) : IntPtr.Zero;
        }

        public static bool ReleaseContext(IntPtr hwnd, IntPtr immContext)
        {
            if (hwnd != IntPtr.Zero && immContext != IntPtr.Zero)
                return Imm32.ImmReleaseContext(hwnd, immContext);
            return false;
        }

        public static void EnableImmComposition()
        {
            if (_threadMgrFailed)
                return;
            if (_threadMgr == null)
            {
                Msctf.TF_CreateThreadMgr(out _threadMgr);
                if (_threadMgr == null)
                {
                    _threadMgrFailed = true;
                    return;
                }
            }
            _threadMgr.SetFocus(IntPtr.Zero);
        }

        public static void SetNoTopmost(Visual visual)
        {
            if (visual == null)
                return;
            if (!(PresentationSource.FromVisual(visual) is HwndSource hwndSource))
                return;
            User32.SetWindowPos(hwndSource.Handle, new IntPtr(-2), 0, 0, 0, 0, 19);
        }

        public static Rect GetScreenRect(Point screenCoordinates)
        {
            var hMonitor = User32.MonitorFromPoint(new NativeMethods.NativeMethods.POINT()
            {
                X = (int)screenCoordinates.X,
                Y = (int)screenCoordinates.Y
            }, 2);
            var structure = new NativeMethods.NativeMethods.Monitorinfo();
            structure.CbSize = Marshal.SizeOf(structure);
            ref var local = ref structure;
            if (User32.GetMonitorInfo(hMonitor, ref local))
                return new Rect(new Point(structure.RcWork.Left, structure.RcWork.Top), new Point(structure.RcWork.Right, structure.RcWork.Bottom));
            return SystemParameters.WorkArea;
        }

        public static bool BrushesEqual(Brush brush, Brush other)
        {
            if (brush == null || other == null)
                return brush == other;
            if (brush.Opacity == 0.0 && other.Opacity == 0.0)
                return true;
            if (!(brush is SolidColorBrush solidColorBrush1) || !(other is SolidColorBrush solidColorBrush2))
                return brush.Equals(other);
            var color = solidColorBrush1.Color;
            if (color.A == 0)
            {
                color = solidColorBrush2.Color;
                if (color.A == 0)
                    return true;
            }
            if (solidColorBrush1.Color == solidColorBrush2.Color)
                return Math.Abs(solidColorBrush1.Opacity - solidColorBrush2.Opacity) < 0.01;
            return false;
        }

        public static bool ImmNotifyIme(IntPtr immContext, int dwAction, int dwIndex, int dwValue)
        {
            return Imm32.ImmNotifyIME(immContext, dwAction, dwIndex, dwValue);
        }

        public static string GetImmCompositionString(IntPtr immContext, int dwIndex)
        {
            if (immContext == IntPtr.Zero)
                return null;
            var compositionStringW1 = Imm32.ImmGetCompositionStringW(immContext, dwIndex, null, 0);
            if (compositionStringW1 <= 0)
                return null;
            var lpBuf = new StringBuilder(compositionStringW1 / 2);
            var compositionStringW2 = Imm32.ImmGetCompositionStringW(immContext, dwIndex, lpBuf, compositionStringW1);
            if (compositionStringW2 <= 0)
                return null;
            return lpBuf.ToString().Substring(0, compositionStringW2 / 2);
        }

        public static Visual GetRootVisual(Visual visual)
        {
            if (visual == null)
                throw new ArgumentNullException(nameof(visual));
            DependencyObject reference = visual;
            var visual1 = visual;
            while ((reference = VisualTreeHelper.GetParent(reference)) != null)
            {
                if (reference is Visual visual2)
                    visual1 = visual2;
            }
            return visual1;
        }

        public static bool SetCompositionPositionAndHeight(HwndSource source, IntPtr immContext, string baseFont, string compositionFont, double topPaddingOverride, double bottomPaddingOverride, double heightPaddingOverride, Point compositionTopLeft, double textHeight, Visual relativeTo, Point viewTopLeft, Point viewBottomRight)
        {
            if (immContext == IntPtr.Zero)
                throw new ArgumentNullException(nameof(immContext));
            if (relativeTo == null)
                throw new ArgumentNullException(nameof(relativeTo));
            var rootVisual = GetRootVisual(relativeTo);
            if (rootVisual == null)
                return false;
            var ancestor = relativeTo.TransformToAncestor(rootVisual);
            if (string.IsNullOrEmpty(compositionFont))
                compositionFont = CompositionFontMapper.GetCompositionFont(baseFont);
            CompositionFontMapper.GetSizeAdjustments(baseFont, compositionFont, out var topPadding, out var bottomPadding, out var heightPadding);
            var num1 = double.IsNaN(topPaddingOverride) ? topPadding : topPaddingOverride;
            bottomPadding = double.IsNaN(bottomPaddingOverride) ? bottomPadding : bottomPaddingOverride;
            var num2 = double.IsNaN(heightPaddingOverride) ? heightPadding : heightPaddingOverride;
            var point = ancestor.Transform(new Point(compositionTopLeft.X, compositionTopLeft.Y + textHeight + bottomPadding));
            compositionTopLeft = ancestor.Transform(new Point(compositionTopLeft.X, compositionTopLeft.Y - num1));
            viewTopLeft = ancestor.Transform(viewTopLeft);
            viewBottomRight = ancestor.Transform(viewBottomRight);
            if (source?.CompositionTarget != null)
            {
                var transformToDevice = source.CompositionTarget.TransformToDevice;
                compositionTopLeft = transformToDevice.Transform(compositionTopLeft);
                point = transformToDevice.Transform(point);
                viewTopLeft = transformToDevice.Transform(viewTopLeft);
                viewBottomRight = transformToDevice.Transform(viewBottomRight);
            }

            var structure1 =
                new NativeMethods.NativeMethods.Logfont
                {
                    lfHeight = (int) Math.Round(point.Y - compositionTopLeft.Y + num2),
                    lfFaceName = compositionFont
                };
            var num3 = Marshal.AllocHGlobal(Marshal.SizeOf(structure1));
            bool flag;
            try
            {
                Marshal.StructureToPtr(structure1, num3, true);
                flag = Imm32.ImmSetCompositionFontW(immContext, num3);
            }
            finally
            {
                Marshal.FreeHGlobal(num3);
            }

            var structure2 = new NativeMethods.NativeMethods.Compositionform
            {
                DwStyle = 1,
                PtCurrentPos = new NativeMethods.NativeMethods.POINT
                {
                    X = Math.Max(0, (int) compositionTopLeft.X),
                    Y = Math.Max(0, (int) compositionTopLeft.Y)
                }
            };
            structure2.RcArea.Left = Math.Min((int)viewTopLeft.X, structure2.PtCurrentPos.X);
            structure2.RcArea.Top = Math.Min((int)viewTopLeft.Y, structure2.PtCurrentPos.Y);
            structure2.RcArea.Right = Math.Max((int)viewBottomRight.X, structure2.PtCurrentPos.X);
            structure2.RcArea.Bottom = Math.Max((int)viewBottomRight.Y, structure2.PtCurrentPos.Y);
            var num4 = Marshal.AllocHGlobal(Marshal.SizeOf(structure2));
            try
            {
                Marshal.StructureToPtr(structure2, num4, true);
                return Imm32.ImmSetCompositionWindow(immContext, num4) & flag;
            }
            finally
            {
                Marshal.FreeHGlobal(num4);
            }
        }


        private static class CompositionFontMapper
        {
            private static readonly IDictionary<int, LanguageFontMapping> LanguageMap =
                new Dictionary<int, LanguageFontMapping>(9);

            private static readonly IDictionary<string, FontSizeMapping> FontMap =
                new Dictionary<string, FontSizeMapping>(25);

            private static readonly IDictionary<string, FontSizeMapping> ConsolasFontMap =
                new Dictionary<string, FontSizeMapping>(6);

            private static readonly IDictionary<string, FontSizeMapping> CourierNewFontMap =
                new Dictionary<string, FontSizeMapping>(6);

            static CompositionFontMapper()
            {
                var languageFontMapping1 = new LanguageFontMapping("SimSun", "Microsoft YaHei");
                var languageFontMapping2 = new LanguageFontMapping("MingLiU", "Microsoft JhengHei");
                var languageFontMapping3 = new LanguageFontMapping("MS Gothic", "Meiryo");
                LanguageMap.Add(4, languageFontMapping1);
                LanguageMap.Add(2052, languageFontMapping1);
                LanguageMap.Add(4100, languageFontMapping1);
                LanguageMap.Add(31748, languageFontMapping2);
                LanguageMap.Add(3076, languageFontMapping2);
                LanguageMap.Add(5124, languageFontMapping2);
                LanguageMap.Add(1028, languageFontMapping2);
                LanguageMap.Add(17, languageFontMapping3);
                LanguageMap.Add(1041, languageFontMapping3);
                ConsolasFontMap.Add("SimSun",
                    new FontSizeMapping(-2.0, 2.0, -2.0));
                ConsolasFontMap.Add("SimSun-ExtB",
                    new FontSizeMapping(-2.0, 2.0, -2.0));
                ConsolasFontMap.Add("Microsoft YaHei",
                    new FontSizeMapping(1.0, 2.0, 2.0));
                ConsolasFontMap.Add("MingLiU",
                    new FontSizeMapping(-2.0, 2.0, -3.0));
                ConsolasFontMap.Add("Microsoft JhengHei",
                    new FontSizeMapping(2.0, 2.0, 3.0));
                ConsolasFontMap.Add("MS Gothic",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                ConsolasFontMap.Add("Meiryo",
                    new FontSizeMapping(1.0, 2.0, 4.0));
                CourierNewFontMap.Add("SimSun",
                    new FontSizeMapping(-2.0, 2.0, -3.0));
                CourierNewFontMap.Add("SimSun-ExtB",
                    new FontSizeMapping(-2.0, 2.0, -3.0));
                CourierNewFontMap.Add("Microsoft YaHei",
                    new FontSizeMapping(1.0, 2.0, 2.0));
                CourierNewFontMap.Add("MingLiU",
                    new FontSizeMapping(-2.0, 2.0, -4.0));
                CourierNewFontMap.Add("Microsoft JhengHei",
                    new FontSizeMapping(2.0, 2.0, 2.0));
                CourierNewFontMap.Add("MS Gothic",
                    new FontSizeMapping(-1.0, 2.0, -3.0));
                CourierNewFontMap.Add("Meiryo",
                    new FontSizeMapping(2.0, 2.0, 4.0));
                FontMap.Add("MS Gothic",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("MS PGothic",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("MS UI Gothic",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("Meiryo",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("Arial Unicode MS",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("MS Mincho",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("MS PMincho",
                    new FontSizeMapping(0.0, 2.0, 0.0));
                FontMap.Add("Dotum",
                    new FontSizeMapping(0.0, 2.0, 1.0));
                FontMap.Add("DotumChe",
                    new FontSizeMapping(0.0, 2.0, 1.0));
                FontMap.Add("Malgun Gothic",
                    new FontSizeMapping(1.0, 2.0, 0.0));
                FontMap.Add("Batang",
                    new FontSizeMapping(0.0, 2.0, -1.0));
                FontMap.Add("BatangChe",
                    new FontSizeMapping(0.0, 2.0, -1.0));
                FontMap.Add("Gulim",
                    new FontSizeMapping(-1.0, 2.0, -1.0));
                FontMap.Add("GulimChe",
                    new FontSizeMapping(-1.0, 2.0, -1.0));
                FontMap.Add("Gungsuh",
                    new FontSizeMapping(-1.0, 2.0, -1.0));
                FontMap.Add("GungsuhChe",
                    new FontSizeMapping(-1.0, 2.0, -1.0));
                FontMap.Add("SimSun",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                FontMap.Add("SimSun-ExtB",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                FontMap.Add("NSimSun",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                FontMap.Add("Microsoft YaHei",
                    new FontSizeMapping(-1.0, 2.0, 0.0));
                FontMap.Add("SimHei",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                FontMap.Add("KaiTi",
                    new FontSizeMapping(-1.0, 2.0, -1.0));
                FontMap.Add("FangSong",
                    new FontSizeMapping(-1.0, 2.0, -2.0));
                FontMap.Add("MingLiU",
                    new FontSizeMapping(-2.0, 1.0, -3.0));
                FontMap.Add("PMingLiU",
                    new FontSizeMapping(-2.0, 1.0, -3.0));
                FontMap.Add("Microsoft JhengHei",
                    new FontSizeMapping(-1.0, 2.0, 0.0));
            }

            public static string GetCompositionFont(string baseFont)
            {
                if (FontMap.ContainsKey(baseFont))
                    return baseFont;
                var lcid = InputLanguageManager.Current.CurrentInputLanguage.LCID;
                if (LanguageMap.TryGetValue(lcid, out var languageFontMapping))
                    return languageFontMapping.GetCompositionFont(Environment.OSVersion.Version.Major);
                return string.Empty;
            }

            public static void GetSizeAdjustments(string baseFont, string compositionFont, out double topPadding,
                out double bottomPadding, out double heightPadding)
            {
                if ((baseFont != "Consolas"
                        ? (baseFont != "Courier New"
                            ? FontMap
                            : CourierNewFontMap)
                        : ConsolasFontMap)
                    .TryGetValue(compositionFont, out var fontSizeMapping))
                {
                    topPadding = fontSizeMapping.TopPadding;
                    bottomPadding = fontSizeMapping.BottomPadding;
                    heightPadding = fontSizeMapping.HeightPadding;
                }
                else
                {
                    topPadding = 0.0;
                    bottomPadding = 2.0;
                    heightPadding = -2.0;
                }
            }

            private struct FontSizeMapping
            {
                public readonly double TopPadding;
                public readonly double BottomPadding;
                public readonly double HeightPadding;

                public FontSizeMapping(double topPadding, double bottomPadding, double unadjustedHeightPadding)
                {
                    TopPadding = topPadding;
                    BottomPadding = bottomPadding;
                    HeightPadding = unadjustedHeightPadding - (topPadding + bottomPadding);
                }
            }

            private class LanguageFontMapping
            {
                private readonly string _oldFallbackFont;
                private readonly string _newFallbackFont;

                public LanguageFontMapping(string oldFallbackFont, string newFallbackFont)
                {
                    _oldFallbackFont = oldFallbackFont;
                    _newFallbackFont = newFallbackFont;
                }

                public string GetCompositionFont(int majorVersion)
                {
                    if (majorVersion < 6)
                        return _oldFallbackFont;
                    return _newFallbackFont;
                }
            }
        }
    }
}