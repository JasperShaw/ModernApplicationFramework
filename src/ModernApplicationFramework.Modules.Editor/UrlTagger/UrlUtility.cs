using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    internal static class UrlUtility
    {
        private static readonly string OpenChars = "\"'(<>[{«༺༼‘“‹⁅⁽₍〈〈《「『【〔〖﴾︵︷︹︻︽︿﹁﹃﹙﹛﹝（［｛｢";
        private static readonly string CloseChars = "\"')><]}»༻༽’”›⁆⁾₎〉〉》」』】〕〗﴿︶︸︺︼︾﹀﹂﹄﹚﹜﹞）］｝｣";
        private static readonly Dictionary<string, bool> StandardProtocols = new Dictionary<string, bool>
        {
            {
                "MK",
                false
            },
            {
                "FTP",
                true
            },
            {
                "MIC",
                true
            },
            {
                "RES",
                true
            },
            {
                "FILE",
                false
            },
            {
                "HTTP",
                true
            },
            {
                "LDAP",
                true
            },
            {
                "NNTP",
                false
            },
            {
                "NEWS",
                false
            },
            {
                "WAIS",
                false
            },
            {
                "LDAPS",
                true
            },
            {
                "SHELL",
                false
            },
            {
                "SNEWS",
                false
            },
            {
                "HTTPS",
                true
            },
            {
                "MAILTO",
                false
            },
            {
                "GOPHER",
                true
            },
            {
                "NEWSRC",
                true
            },
            {
                "RLOGIN",
                false
            },
            {
                "TELNET",
                false
            },
            {
                "OUTLOOK",
                false
            },
            {
                "LOCAL",
                true
            },
            {
                "ABOUT",
                false
            }
        };
        private static readonly object CachedProtocolMutex = new object();
        private static HashSet<string> _cachedProtocols;

        internal static IEnumerable<UrlSpan> FindUrLs(string text, int? hitOffset)
        {
            var end = -1;
            var lastColonIndex = -1;
            var needToUpperCase = true;
            while ((end = text.IndexOf(':', end + 1)) != -1)
            {
                if (end > lastColonIndex + 2)
                {
                    if (end == text.Length - 1)
                        break;
                    if (needToUpperCase)
                    {
                        needToUpperCase = false;
                        text = text.ToUpperInvariant();
                    }
                    var index1 = -1;
                    int startIndex;
                    for (startIndex = end - 1; startIndex > lastColonIndex; --startIndex)
                    {
                        var ch = text[startIndex];
                        if (!IsProtocolChar(ch))
                        {
                            TryGetOpenCharIndex(ch, out index1);
                            break;
                        }
                    }
                    lastColonIndex = end;
                    if (hitOffset.HasValue && startIndex > hitOffset.Value)
                        break;
                    ++startIndex;
                    if (IsProtocol(text.Substring(startIndex, Math.Min(text.Length - startIndex, end - startIndex + 3))))
                    {
                        var addressStart = end + 1;
                        if (!IsInvalidUrlCharacter(text[addressStart]))
                        {
                            var lastCharInUrl = -1;
                            if (index1 != -1)
                            {
                                lastCharInUrl = text.IndexOf(CloseChars[index1], addressStart);
                                if (lastCharInUrl != -1)
                                {
                                    for (var index2 = addressStart; index2 < lastCharInUrl; ++index2)
                                    {
                                        if (IsIllegalUrlChar(text[index2]))
                                        {
                                            lastCharInUrl = index2;
                                            break;
                                        }
                                    }
                                    if (!hitOffset.HasValue || startIndex - 1 <= hitOffset.Value && hitOffset.Value <= lastCharInUrl)
                                    {
                                        yield return new UrlSpan
                                        {
                                            Url = Span.FromBounds(startIndex, lastCharInUrl),
                                            Address = Span.FromBounds(addressStart, lastCharInUrl),
                                            Protocol = Span.FromBounds(startIndex, end)
                                        };
                                        end = lastCharInUrl - 1;
                                        continue;
                                    }
                                }
                            }
                            lastCharInUrl = addressStart;
                            while (lastCharInUrl < text.Length && !IsIllegalUrlChar(text[lastCharInUrl]))
                                ++lastCharInUrl;
                            for (--lastCharInUrl; lastCharInUrl > end; --lastCharInUrl)
                            {
                                var c = text[lastCharInUrl];
                                switch (c)
                                {
                                    case '/':
                                    case '\\':
                                        goto label_32;
                                    default:
                                        if (char.IsPunctuation(c))
                                            continue;
                                        goto label_32;
                                }
                            }
                            label_32:
                            if (lastCharInUrl != end && (!hitOffset.HasValue || startIndex <= hitOffset.Value && hitOffset.Value <= lastCharInUrl))
                            {
                                yield return new UrlSpan
                                {
                                    Url = Span.FromBounds(startIndex, lastCharInUrl + 1),
                                    Address = Span.FromBounds(addressStart, lastCharInUrl + 1),
                                    Protocol = Span.FromBounds(startIndex, end)
                                };
                                end = lastCharInUrl;
                            }
                        }
                    }
                }
            }
        }

        private static bool IsInvalidUrlCharacter(char c)
        {
            if (c != '<' && c != '>' && c != '"')
                return c == '\'';
            return true;
        }

        private static bool IsIllegalUrlChar(char c)
        {
            if (c != '"' && c != '\'' && (c != '\t' && c != '\x200B'))
                return char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
            return true;
        }

        private static bool TryGetOpenCharIndex(char openChar, out int index)
        {
            index = OpenChars.IndexOf(openChar);
            return index != -1;
        }

        private static HashSet<string> CachedProtocols
        {
            get
            {
                if (_cachedProtocols == null)
                {
                    lock (CachedProtocolMutex)
                        _cachedProtocols = LoadProtocolsFromRegistry();
                }
                return _cachedProtocols;
            }
        }

        private static HashSet<string> LoadProtocolsFromRegistry()
        {
            var stringSet = new HashSet<string>();
            using (var registryKey = Registry.ClassesRoot.OpenSubKey("PROTOCOLS\\Handler", false))
            {
                foreach (var key in registryKey.GetSubKeyNames().Select(s => s.ToUpperInvariant()))
                {
                    if (!StandardProtocols.ContainsKey(key))
                        stringSet.Add(key);
                }
            }
            return stringSet;
        }

        private static bool IsProtocolChar(char c)
        {
            if (!char.IsLetter(c) && c != '.' && c != '+')
                return c == '-';
            return true;
        }

        private static bool IsProtocol(string text)
        {
            var foundSlashes = false;
            var num = Math.Min(64, text.Length);
            int length;
            for (length = 0; length < num; ++length)
            {
                if (text[length] == ':')
                {
                    foundSlashes = length < text.Length - 2 && text[length + 1] == '/' && text[length + 2] == '/';
                    break;
                }
            }
            if (length < 2)
                return false;
            var possibleProtocol = text.Substring(0, length);
            switch (IsStandardProtocol(possibleProtocol, foundSlashes))
            {
                case ValidProtocolFound.ValidProtocol:
                    return true;
                case ValidProtocolFound.ValidProtocolNoSlash:
                    return false;
                default:
                    if (foundSlashes)
                        return CachedProtocols.Contains(possibleProtocol);
                    return false;
            }
        }

        private static ValidProtocolFound IsStandardProtocol(string possibleProtocol, bool foundSlashes)
        {
            if (possibleProtocol.Length < 2 || possibleProtocol.Length > 7 || !StandardProtocols.TryGetValue(possibleProtocol, out var flag))
                return ValidProtocolFound.InvalidProtocol;
            return flag && !foundSlashes ? ValidProtocolFound.ValidProtocolNoSlash : ValidProtocolFound.ValidProtocol;
        }
    }
}