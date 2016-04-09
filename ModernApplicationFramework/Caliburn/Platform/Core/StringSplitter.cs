using System.Collections.Generic;
using System.Text;

namespace ModernApplicationFramework.Caliburn.Platform.Core
{
    /// <summary>
    ///     Helper class when splitting strings
    /// </summary>
    public static class StringSplitter
    {
        /// <summary>
        ///     Splits a string with a chosen separator.
        ///     If a substring is contained in [...] it will not be splitted.
        /// </summary>
        /// <param name="message">The message to split</param>
        /// <param name="separator">The separator to use when splitting</param>
        /// <returns></returns>
        public static string[] Split(string message, char separator)
        {
            //Splits a string using the specified separator, if it is found outside of relevant places
            //delimited by [ and ]
            string str;
            var list = new List<string>();
            var builder = new StringBuilder();

            var squareBrackets = 0;

            foreach (var current in message)
            {
                //Square brackets are used as delimiters, so only separators outside them count...
                switch (current)
                {
                    case '[':
                        squareBrackets++;
                        break;
                    case ']':
                        squareBrackets--;
                        break;
                    default:
                        if (current == separator)
                        {
                            if (squareBrackets == 0)
                            {
                                str = builder.ToString();
                                if (!string.IsNullOrEmpty(str))
                                    list.Add(builder.ToString());
                                builder.Length = 0;
                                continue;
                            }
                        }
                        break;
                }

                builder.Append(current);
            }

            str = builder.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                list.Add(builder.ToString());
            }

            return list.ToArray();
        }

        /// <summary>
        ///     Splits a string with , as separator.
        ///     Does not split within {},[],()
        /// </summary>
        /// <param name="parameters">The string to split</param>
        /// <returns></returns>
        public static string[] SplitParameters(string parameters)
        {
            //Splits parameter string taking into account brackets...
            var list = new List<string>();
            var builder = new StringBuilder();

            var isInString = false;

            var curlyBrackets = 0;
            var squareBrackets = 0;
            var roundBrackets = 0;
            for (var i = 0; i < parameters.Length; i++)
            {
                var current = parameters[i];

                if (current == '"')
                {
                    if (i == 0 || parameters[i - 1] != '\\')
                    {
                        isInString = !isInString;
                    }
                }

                if (!isInString)
                {
                    switch (current)
                    {
                        case '{':
                            curlyBrackets++;
                            break;
                        case '}':
                            curlyBrackets--;
                            break;
                        case '[':
                            squareBrackets++;
                            break;
                        case ']':
                            squareBrackets--;
                            break;
                        case '(':
                            roundBrackets++;
                            break;
                        case ')':
                            roundBrackets--;
                            break;
                        default:
                            if (current == ',' && roundBrackets == 0 && squareBrackets == 0 && curlyBrackets == 0)
                            {
                                //The only commas to be considered as parameter separators are outside:
                                //- Strings
                                //- Square brackets (to ignore indexers)
                                //- Parantheses (to ignore method invocations)
                                //- Curly brackets (to ignore initializers and Bindings)
                                list.Add(builder.ToString());
                                builder.Length = 0;
                                continue;
                            }
                            break;
                    }
                }

                builder.Append(current);
            }

            list.Add(builder.ToString());

            return list.ToArray();
        }
    }
}