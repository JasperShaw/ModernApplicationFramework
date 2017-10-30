using System;
using System.Text;

namespace ModernApplicationFramework.Utilities
{
    public static class Accelerator
    {
        public static string StripAccelerators(string input)
        {
            return StripAccelerators(input, '&');
        }

        public static string StripAccelerators(string input, char accessSpecifier)
        {
            if (string.IsNullOrEmpty(input) || input.IndexOf(accessSpecifier) < 0)
                return input;
            using (ReusableResourceHolder<StringBuilder> reusableResourceHolder = ReusableStringBuilder.AcquireDefault(input.Length))
            {
                StringBuilder resource = reusableResourceHolder.Resource;
                for (int index1 = 0; index1 < input.Length; ++index1)
                {
                    if (input[index1] == accessSpecifier)
                        ++index1;
                    else if (input[index1] == 40)
                    {
                        bool flag = false;
                        int num = -1;
                        for (int index2 = index1 + 1; index2 < input.Length; ++index2)
                        {
                            if (input[index2] == 41)
                            {
                                num = index2;
                                break;
                            }
                            if (input[index2] == accessSpecifier)
                            {
                                if (index2 == input.Length - 1 || input[index2 + 1] == accessSpecifier)
                                    ++index2;
                                else
                                    flag = true;
                            }
                        }
                        if (flag && num >= 0)
                        {
                            index1 = num;
                            continue;
                        }
                    }
                    if (index1 < input.Length)
                        resource.Append(input[index1]);
                }
                return resource.ToString().TrimEnd(Array.Empty<char>());
            }
        }

        public static string StripAccelerators(string input, object accessKeySpecifier)
        {
            return StripAccelerators(input, AccessKeySpecifierFromObject(accessKeySpecifier));
        }

        public static char AccessKeySpecifierFromObject(object accessKeySpecifier)
        {
            var ch = '&';
            if (accessKeySpecifier is char)
            {
                ch = (char)accessKeySpecifier;
            }
            else
            {
                string str = accessKeySpecifier as string;
                if (str != null && str.Length == 1)
                    ch = str[0];
            }
            return ch;
        }
    }
}
