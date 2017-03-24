using System.Globalization;
using System.Windows.Documents;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Core.Converters
{
    public class AccessKeyUnderliningConverter : ValueConverter<string, object>
    {
        protected override object Convert(string fullText, object parameter, CultureInfo culture)
        {
            char ch1 = Accelerator.AccessKeySpecifierFromObject(parameter);
            var span = new Span();
            while (!string.IsNullOrEmpty(fullText))
            {
                int length = fullText.IndexOf(ch1);
                if (length == -1)
                {
                    span.Inlines.Add(new Run(fullText));
                    fullText = string.Empty;
                }
                else
                {
                    if (length > 0)
                        span.Inlines.Add(new Run(fullText.Substring(0, length)));
                    if (length < fullText.Length - 1)
                    {
                        char ch2 = fullText[length + 1];
                        Run run = new Run(ch2.ToString());
                        if (ch2 == ch1)
                            span.Inlines.Add(run);
                        else
                            span.Inlines.Add(new Underline(run));
                        fullText = fullText.Substring(length + 2);
                    }
                    else
                        fullText = string.Empty;
                }
            }
            return span;
        }
    }
}
