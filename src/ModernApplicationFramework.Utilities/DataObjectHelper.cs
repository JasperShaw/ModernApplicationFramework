using System.Linq;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Utilities
{
    public static class DataObjectHelper
    {
        private static readonly string[] KnownStringFormats = {
            System.Windows.DataFormats.StringFormat,
            System.Windows.DataFormats.UnicodeText,
            System.Windows.DataFormats.Text,
            System.Windows.DataFormats.OemText
        };

        public static bool DataObjectHasString(System.Windows.IDataObject source)
        {
            return KnownStringFormats.Any(source.GetDataPresent);
        }

        public static string GetStringFromDataObject(System.Windows.IDataObject source)
        {
            foreach (var knownStringFormat in KnownStringFormats)
            {
                if (source.GetDataPresent(knownStringFormat))
                {
                    string str = null;
                    try
                    {
                        str = source.GetData(knownStringFormat) as string;
                    }
                    catch (COMException)
                    {
                    }
                    if (str != null)
                        return str;
                }
            }
            return null;
        }
    }
}
