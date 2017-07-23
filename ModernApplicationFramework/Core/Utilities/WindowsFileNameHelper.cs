using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Core.Utilities
{
    /// <summary>
    /// A helper class that adds rich functions for Windows OS file paths
    /// </summary>
    public static class WindowsFileNameHelper
    {
        private static readonly Regex RegexInvalidname =
            new Regex("^((\\..*)|COM\\d|CLOCK\\$|LPT\\d|AUX|NUL|CON|PRN|(.*[\\ud800-\\udfff]+.*))$",
                RegexOptions.IgnoreCase);

        private static readonly Regex RegexInvalidpath =
            new Regex("(^|\\\\)(AUX|CLOCK\\$|LPT|NUL|CON|COM\\d{1}|LPT\\d{1}|(.*[\\ud800-\\udfff]+.*))(\\\\|$)",
                RegexOptions.IgnoreCase);

        private static readonly char[] InvalidNamechars = "/?:&\\*\"<>|#%".ToCharArray();

        public static bool IsValidDrive(string location)
        {
            if (!Path.IsPathRooted(location))
                return true;
            var nDrive = Path.GetPathRoot(location) ?? "";
            if (
                !nDrive.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                    StringComparison.CurrentCultureIgnoreCase))
                nDrive += Path.DirectorySeparatorChar;
            switch ((DriveType) Kernel32.GetDriveTypeW(nDrive))
            {
                case DriveType.Fixed:
                case DriveType.Removable:
                case DriveType.Network:
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static bool IsValidFileName(string strInput)
        {
            return !string.IsNullOrEmpty(strInput) && !RegexInvalidname.IsMatch(strInput) &&
                   strInput.IndexOfAny(InvalidNamechars) < 0;
        }

        public static bool IsValidPath(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            try
            {
                if (!IsValidDrive(input))
                    return false;
                Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
                if (!driveCheck.IsMatch(input.Substring(0, 3))) return false;
                string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
                strTheseAreInvalidFileNameChars += @":/?*" + "\"";
                Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
                if (containsABadCharacter.IsMatch(input.Substring(3, input.Length - 3)))
                    return false;
                if (RegexInvalidpath.IsMatch(input))
                    return false;
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }
    }
}