using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using ModernApplicationFramework.Editor.NativeMethods;

namespace ModernApplicationFramework.Editor.Interop
{
    public static class InstalledFontHelper
    {
        private static readonly InstalledFontCollection _fontsCollection;
        private static readonly HashSet<FontFamily> _monospaceFontFamilies = new HashSet<FontFamily>();

        static InstalledFontHelper()
        {
            _fontsCollection = new InstalledFontCollection();
        }

        public static IReadOnlyCollection<string> GetInstalledFontNames()
        {
            return _fontsCollection.Families.Select(fontFamily => fontFamily.ToString()).ToList();
        }

        public static IReadOnlyCollection<FontFamily> GetInstalledFonts()
        {
            return _fontsCollection.Families;
        }

        public static IReadOnlyCollection<FontFamily> GetInstalledMonospaceFonts()
        {
            _monospaceFontFamilies.Clear();
            var graphics = Graphics.FromHwnd(IntPtr.Zero);
            var hdc = graphics.GetHdc();
            var logfont = new Logfont { CharSet = 1 };
            Gdi32.EnumFontFamiliesEx(hdc, ref logfont, FilterMonospace, IntPtr.Zero, 0);
            return _monospaceFontFamilies;
        }

        private static int FilterMonospace(ref NativeMethods.NativeMethods.EnumLogFont lpelf, ref NativeMethods.NativeMethods.NewTextMetric lpntm, uint fonttype, IntPtr lparam)
        {
            if ((lpelf.LogFont.PitchAndFamily & 0x3) == 1)
            {
                var logfont = lpelf.LogFont;
                var fontFamiliy = GetFamily(logfont.FaceName);
                if (fontFamiliy != null)
                    _monospaceFontFamilies.Add(fontFamiliy);
            }
            return 1;
        }

        public static FontFamily GetFamily(string name)
        {
            return _fontsCollection.Families.FirstOrDefault(x =>
                x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsSystemFontAvailable(string str)
        {
            return _fontsCollection.Families.Any(fontFamiliy => fontFamiliy.Name == str);
        }
    }
}
