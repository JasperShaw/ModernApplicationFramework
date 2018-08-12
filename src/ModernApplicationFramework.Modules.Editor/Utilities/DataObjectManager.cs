using System;
using System.IO;
using System.Text;
using System.Windows;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal static class DataObjectManager
    {
        public static bool ContainsText(IDataObject data)
        {
            if (!data.GetDataPresent(DataFormats.Text, true) && !data.GetDataPresent(DataFormats.Html))
                return data.GetDataPresent(DataFormats.CommaSeparatedValue);
            return true;
        }

        public static string ExtractText(IDataObject data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.GetDataPresent(DataFormats.UnicodeText))
                return (string) data.GetData(DataFormats.UnicodeText);
            if (data.GetDataPresent(DataFormats.Text))
                return (string) data.GetData(DataFormats.Text);
            if (data.GetDataPresent(DataFormats.Html))
                return ExtractHTMLText(data).Trim();
            if (data.GetDataPresent(DataFormats.CommaSeparatedValue))
                return (string) data.GetData(DataFormats.Text, true);
            return string.Empty;
        }

        internal static string ExtractHTMLText(IDataObject data)
        {
            var data1 = data.GetData(DataFormats.Html) as string;
            string str;
            if (data1 != null)
            {
                str = data1;
            }
            else
            {
                MemoryStream data2;
                try
                {
                    data2 = (MemoryStream) data.GetData(DataFormats.Html);
                }
                catch
                {
                    throw new InvalidOperationException(
                        "Can't examine data in IDataObject object, MemoryStream expected but not found");
                }

                var decoder = Encoding.UTF8.GetDecoder();
                var array = data2.ToArray();
                var charCount = decoder.GetCharCount(array, 0, array.Length);
                var chars = new char[charCount];
                int bytesUsed;
                int charsUsed;
                bool completed;
                decoder.Convert(array, 0, array.Length, chars, 0, charCount, true, out bytesUsed, out charsUsed,
                    out completed);
                str = new string(chars);
            }

            var num1 = str.IndexOf("<!--StartFragment-->", StringComparison.CurrentCultureIgnoreCase);
            var num2 = str.IndexOf("<!--EndFragment-->", StringComparison.CurrentCultureIgnoreCase);
            if (num1 != -1 && num2 != -1 && num2 > num1)
                return str.Substring(num1 + "<!--StartFragment-->".Length, num2 - num1 - "<!--StartFragment-->".Length);
            return string.Empty;
        }
    }
}