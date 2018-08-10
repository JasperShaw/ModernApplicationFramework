using System.Globalization;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Utilities
{
    public class UnicodeWordExtent
    {
        public const char UchCr = '\r';
        public const char UchHangulJamoFirst = 'ᄀ';
        public const char UchHangulJamoLast = 'ᇿ';
        public const char UchHangulJamoLeadFirst = 'ᄀ';
        public const char UchHangulJamoLeadLast = 'ᅟ';
        public const char UchHangulJamoTrailFirst = 'ᆨ';
        public const char UchHangulJamoTrailLast = 'ᇹ';
        public const char UchHangulJamoVowelFirst = 'ᅠ';
        public const char UchHangulJamoVowelLast = 'ᆢ';
        public const char UchLf = '\n';
        public const char UchLs = '\x2028';
        public const char UchNel = '\x0085';
        public const char UchPs = '\x2029';

        public enum HangulJamoType
        {
            Other,
            Lead,
            Vowel,
            Trail
        }

        public enum UnicodeScript
        {
            None,
            Latin,
            Greek,
            Cyrillic,
            Armenian,
            Hebrew,
            Arabic,
            Devanagari,
            Bangla,
            Gurmukhi,
            Gujarati,
            Odia,
            Tamil,
            Telugu,
            Kannada,
            Malayalam,
            Thai,
            Lao,
            Tibetan,
            Georgian,
            Cjk,
            Braille,
            Syriac,
            Thaana,
            Sinhala,
            Myanmar,
            Ethiopic,
            Cherokee,
            CanadianAboriginal,
            Ogham,
            Runic,
            Khmer,
            Mongolian,
            Yi
        }

        public static bool FindCurrentToken(SnapshotPoint currentPosition, out SnapshotSpan span)
        {
            var containingLine = currentPosition.GetContainingLine();
            var line = new LineBuffer(containingLine);
            var includingLineBreak = containingLine.LengthIncludingLineBreak;
            var iChar = currentPosition - containingLine.Start;
            if (iChar >= includingLineBreak)
            {
                span = new SnapshotSpan(currentPosition.Snapshot, containingLine.End, 0);
                return true;
            }

            while (iChar > 0 && !IsGraphemeBreak(line, iChar))
                --iChar;
            var ch1 = line[iChar];
            if (IsWordChar(ch1))
                return FindCurrentWordCoords(new SnapshotPoint(currentPosition.Snapshot, containingLine.Start + iChar),
                    out span);
            int num1;
            int index1;
            if (char.IsWhiteSpace(ch1))
            {
                var index2 = iChar - 1;
                while (index2 >= 0 && char.IsWhiteSpace(line[index2]))
                    --index2;
                num1 = index2 + 1;
                index1 = iChar + 1;
                while (index1 < includingLineBreak && char.IsWhiteSpace(line[index1]))
                    ++index1;
            }
            else if (char.IsPunctuation(ch1) || IsMathSymbol(ch1))
            {
                int index2;
                for (index2 = iChar - 1; index2 >= 0; --index2)
                {
                    var ch2 = line[index2];
                    if (!char.IsPunctuation(ch2) && !IsMathSymbol(ch2))
                        break;
                }

                num1 = index2 + 1;
                for (index1 = iChar + 1; index1 < includingLineBreak; ++index1)
                {
                    var ch2 = line[index1];
                    if (!char.IsPunctuation(ch2) && !IsMathSymbol(ch2))
                        break;
                }
            }
            else if (char.IsHighSurrogate(ch1) && iChar + 1 < includingLineBreak &&
                     char.IsLowSurrogate(line[iChar + 1]))
            {
                num1 = iChar;
                index1 = iChar + 2;
            }
            else
            {
                var num2 = iChar;
                index1 = num2 + 1;
                num1 = num2;
            }

            if (index1 > includingLineBreak)
                index1 = includingLineBreak;
            span = new SnapshotSpan(currentPosition.Snapshot, containingLine.Start + num1, index1 - num1);
            return true;
        }

        public static bool FindCurrentWordCoords(SnapshotPoint currentPosition, out SnapshotSpan span)
        {
            span = new SnapshotSpan(currentPosition, 0);
            var containingLine = currentPosition.GetContainingLine();
            var line = new LineBuffer(containingLine);
            var includingLineBreak = containingLine.LengthIncludingLineBreak;
            var iChar1 = currentPosition - containingLine.Start;
            while (iChar1 > 0 && !IsGraphemeBreak(line, iChar1))
                --iChar1;
            if (iChar1 == includingLineBreak || !IsWordChar(line[iChar1]))
            {
                if (iChar1 == 0 || !IsWordChar(line[iChar1 - 1]))
                    return false;
                --iChar1;
            }

            var iChar2 = iChar1;
            while (iChar2 >= 0 && !IsWordBreak(line, iChar2, false))
                --iChar2;
            var iChar3 = iChar1 + 1;
            while (iChar3 < includingLineBreak && !IsWordBreak(line, iChar3, false))
                ++iChar3;
            span = new SnapshotSpan(currentPosition.Snapshot, containingLine.Start + iChar2, iChar3 - iChar2);
            return true;
        }

        public static HangulJamoType GetHangulJamoType(char ch)
        {
            if (ch < 'ᄀ' || ch > 'ᇿ')
                return HangulJamoType.Other;
            if (ch <= 'ᅟ')
                return HangulJamoType.Lead;
            return ch <= 'ᆢ' ? HangulJamoType.Vowel : HangulJamoType.Trail;
        }

        public static bool IsCombining(char ch)
        {
            return IsPropCombining(char.GetUnicodeCategory(ch));
        }

        public static bool IsGraphemeBreak(LineBuffer line, int iChar)
        {
            if (iChar <= 0)
                return true;
            var ch1 = line[iChar];
            if (ch1 == char.MinValue)
                return true;
            var ch2 = line[iChar - 1];
            if (ch2 == char.MinValue || IsLineBreak(ch2) || IsLineBreak(ch1))
                return true;
            if (IsCombining(ch1) && ch2 != '\'' && ch2 != '"' && !char.IsWhiteSpace(ch2) && char.IsLetter(ch2))
                return false;
            if (char.IsHighSurrogate(ch2))
                return !char.IsLowSurrogate(ch1);
            var hangulJamoType1 = GetHangulJamoType(ch2);
            var hangulJamoType2 = GetHangulJamoType(ch1);
            if (hangulJamoType1 != HangulJamoType.Other && hangulJamoType2 != HangulJamoType.Other)
                switch (hangulJamoType1)
                {
                    case HangulJamoType.Lead:
                        return false;
                    case HangulJamoType.Vowel:
                        return HangulJamoType.Lead == hangulJamoType2;
                    case HangulJamoType.Trail:
                        return HangulJamoType.Trail != hangulJamoType2;
                }
            return true;
        }

        public static bool IsHiragana(char ch)
        {
            if (ch < '〱')
                return false;
            if ((ch < 'ぁ' || ch > 'ゞ') && 'ー' != ch)
                return 'ｰ' == ch;
            return true;
        }

        public static bool IsIdeograph(char ch)
        {
            var flag = false;
            if (ch < '一')
            {
                if (ch == '々')
                    flag = true;
            }
            else if (ch <= '龥')
            {
                flag = true;
            }
            else if (ch >= '豈' && ch <= '鶴')
            {
                flag = true;
            }

            return flag;
        }

        public static bool IsKatakana(char ch)
        {
            if (ch < '〱')
                return false;
            if (ch >= '゙' && ch <= 'ヾ')
                return true;
            if (ch >= 'ｦ')
                return ch <= 'ﾟ';
            return false;
        }

        public static bool IsLineBreak(char ch)
        {
            if (ch > '\x2029')
                return false;
            if ('\r' != ch && '\n' != ch && '\x2028' != ch && '\x2029' != ch)
                return '\x0085' == ch;
            return true;
        }

        public static bool IsMathSymbol(char ch)
        {
            return char.GetUnicodeCategory(ch) == UnicodeCategory.MathSymbol;
        }

        public static bool IsNonBreaking(char ch)
        {
            if (ch != ' ' && ch != '‑' && ch != ' ' && ch != 'ー')
                return ch == '\xFEFF';
            return true;
        }

        public static bool IsPropAlpha(UnicodeCategory cat)
        {
            return cat <= UnicodeCategory.OtherLetter;
        }

        public static bool IsPropBreak(char cL, char cR, bool fHanWordBreak)
        {
            var unicodeCategory1 = char.GetUnicodeCategory(cR);
            if (IsPropCombining(unicodeCategory1))
                return false;
            var flag1 = IsWordChar(cL);
            var flag2 = IsWordChar(cR);
            if (flag1 != flag2 || !flag1 && !flag2)
                return true;
            var unicodeScript1 = UScript(cL);
            var unicodeScript2 = UScript(cR);
            if (unicodeScript1 != unicodeScript2)
            {
                var unicodeCategory2 = char.GetUnicodeCategory(cL);
                if (IsPropAlpha(unicodeCategory2) && IsPropAlpha(unicodeCategory1) ||
                    IsPropDigit(unicodeCategory2) && IsPropDigit(unicodeCategory1))
                    return true;
            }

            if (unicodeScript1 == UnicodeScript.Cjk || unicodeScript2 == UnicodeScript.Cjk)
            {
                var flag3 = IsIdeograph(cL);
                var flag4 = IsIdeograph(cR);
                if (fHanWordBreak && flag3 | flag4 ||
                    unicodeScript1 != UnicodeScript.Cjk && unicodeScript2 == UnicodeScript.Cjk)
                    return true;
                var flag5 = IsHiragana(cR);
                if (IsHiragana(cL) && !flag5 || IsKatakana(cL) && !flag5 && !IsKatakana(cR) ||
                    flag3 && !(flag4 | flag5))
                    return true;
            }

            return false;
        }

        public static bool IsPropCombining(UnicodeCategory cat)
        {
            if (cat >= UnicodeCategory.NonSpacingMark)
                return cat <= UnicodeCategory.EnclosingMark;
            return false;
        }

        public static bool IsPropDigit(UnicodeCategory cat)
        {
            return cat == UnicodeCategory.DecimalDigitNumber;
        }

        public static bool IsWholeWord(SnapshotSpan candidate, bool acceptHanWordBreak = true)
        {
            var containingLine = candidate.Start.GetContainingLine();
            var line = (LineBuffer) null;
            if (containingLine.Extent.End < candidate.End)
                return false;
            var flag1 = candidate.Start == 0;
            var flag2 = candidate.End == candidate.Snapshot.Length;
            if (!flag1 || !flag2)
                line = new LineBuffer(containingLine);
            if (!flag1)
            {
                flag1 = IsWordBreak(line, candidate.Start - containingLine.Start, acceptHanWordBreak);
                if (!flag1)
                    return false;
            }

            if (!flag2)
                flag2 = IsWordBreak(line, candidate.End - containingLine.Start, acceptHanWordBreak);
            return flag1 & flag2;
        }

        public static bool IsWordBreak(LineBuffer line, int iChar, bool fHanWordBreak)
        {
            if (iChar <= 0)
                return true;
            if (!IsGraphemeBreak(line, iChar))
                return false;
            var ch1 = line[iChar];
            var index = PrevChar(line, iChar);
            if (index < 0)
                return true;
            var ch2 = line[index];
            if (IsNonBreaking(ch2) || IsNonBreaking(ch1))
                return false;
            return IsPropBreak(ch2, ch1, fHanWordBreak);
        }

        public static bool IsWordChar(char ch)
        {
            if (!char.IsLetterOrDigit(ch) && ch != '_')
                return ch == '$';
            return true;
        }

        public static int PrevChar(LineBuffer line, int iChar)
        {
            if (iChar <= 0)
                return -1;
            --iChar;
            while (!IsGraphemeBreak(line, iChar))
                --iChar;
            return iChar;
        }

        public static UnicodeScript UScript(char ch)
        {
            if (ch <= 'ɏ')
                return UnicodeScript.Latin;
            if (ch < ' ')
            {
                if (ch < 'က')
                {
                    if (ch < 'Ͱ')
                        return UnicodeScript.None;
                    if (ch < 'Ѐ')
                        return UnicodeScript.Greek;
                    if (ch <= 'ӿ')
                        return UnicodeScript.Cyrillic;
                    if (ch < '\x0530')
                        return UnicodeScript.None;
                    if (ch < '\x0590')
                        return UnicodeScript.Armenian;
                    if (ch < '\x0600')
                        return UnicodeScript.Hebrew;
                    if (ch < '܀')
                        return UnicodeScript.Arabic;
                    if (ch <= 'ݏ')
                        return UnicodeScript.Syriac;
                    if (ch < 'ހ')
                        return UnicodeScript.None;
                    if (ch <= '\x07BF')
                        return UnicodeScript.Thaana;
                    if (ch < 'ऀ')
                        return UnicodeScript.None;
                    if (ch < 'ঀ')
                        return UnicodeScript.Devanagari;
                    if (ch < '\x0A00')
                        return UnicodeScript.Bangla;
                    if (ch < '\x0A80')
                        return UnicodeScript.Gurmukhi;
                    if (ch < '\x0B00')
                        return UnicodeScript.Gujarati;
                    if (ch < '\x0B80')
                        return UnicodeScript.Odia;
                    if (ch < 'ఀ')
                        return UnicodeScript.Tamil;
                    if (ch < '\x0C80')
                        return UnicodeScript.Telugu;
                    if (ch < '\x0D00')
                        return UnicodeScript.Kannada;
                    if (ch < '\x0D80')
                        return UnicodeScript.Malayalam;
                    if (ch < '\x0E00')
                        return UnicodeScript.Sinhala;
                    if (ch < '\x0E80')
                        return UnicodeScript.Thai;
                    return ch < 'ༀ' ? UnicodeScript.Lao : UnicodeScript.Tibetan;
                }

                if (ch < 'Ⴀ')
                    return UnicodeScript.Myanmar;
                if (ch < 'ᄀ')
                    return UnicodeScript.Georgian;
                if (ch < 'ሀ')
                    return UnicodeScript.Cjk;
                if (ch < 'Ꭰ')
                    return UnicodeScript.Ethiopic;
                if (ch < '᐀')
                    return UnicodeScript.Cherokee;
                if (ch < ' ')
                    return UnicodeScript.CanadianAboriginal;
                if (ch < 'ᚠ')
                    return UnicodeScript.Ogham;
                if (ch < 'ក')
                    return UnicodeScript.Runic;
                if (ch < '᠀')
                    return UnicodeScript.Khmer;
                if (ch <= '\x18AF')
                    return UnicodeScript.Mongolian;
                if (ch < 'Ḁ')
                    return UnicodeScript.None;
                return ch < 'ἀ' ? UnicodeScript.Latin : UnicodeScript.Greek;
            }

            if (ch < '\xD800')
            {
                if (ch <= '⟿')
                    return UnicodeScript.None;
                if (ch <= '⣿')
                    return UnicodeScript.Braille;
                if (ch < '⺀')
                    return UnicodeScript.None;
                if (ch <= '\x31BF')
                    return UnicodeScript.Cjk;
                if (ch < '㈀')
                    return UnicodeScript.None;
                if (ch <= '\x4DBF')
                    return UnicodeScript.Cjk;
                if (ch < '一')
                    return UnicodeScript.None;
                if (ch <= '\x9FFF')
                    return UnicodeScript.Cjk;
                if (ch <= '\xA4CF')
                    return UnicodeScript.Yi;
                return ch < '가' || ch > '힣' ? UnicodeScript.None : UnicodeScript.Cjk;
            }

            if (ch < '豈')
                return UnicodeScript.None;
            if (ch < 'ﬀ')
                return UnicodeScript.Cjk;
            if (ch < 'ﭏ')
                return UnicodeScript.Latin;
            if (ch < '︀')
                return UnicodeScript.Arabic;
            if (ch < '︰')
                return UnicodeScript.None;
            if (ch < '﹐')
                return UnicodeScript.Cjk;
            if (ch < 'ﹰ')
                return UnicodeScript.None;
            if (ch < '\xFEFF')
                return UnicodeScript.Arabic;
            if (ch >= '\xFFF0' || ch == '\xFEFF')
                return UnicodeScript.None;
            return ch >= '！' && ch <= '～' ? UnicodeScript.Latin : UnicodeScript.Cjk;
        }
    }
}