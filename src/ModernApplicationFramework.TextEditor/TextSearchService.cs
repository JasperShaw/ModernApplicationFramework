using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextSearchService))]
    [Export(typeof(ITextSearchService2))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class TextSearchService : ITextSearchService2
    {
        private static readonly IDictionary<string, WeakReference> CachedRegexEngines = new Dictionary<string, WeakReference>(10);
        private static readonly ReaderWriterLockSlim RegexCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        [Import]
        private ITextStructureNavigatorSelectorService _navigatorSelectorService;

        public SnapshotSpan? FindNext(int startIndex, bool wraparound, FindData findData)
        {
            if (startIndex < 0 || startIndex > findData.TextSnapshotToSearch.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (string.IsNullOrEmpty(findData.SearchString))
                throw new ArgumentException("Search pattern can't be empty or null", nameof(findData));
            var findOptions = findData.FindOptions;
            if (wraparound)
                findOptions |= FindOptions.Wrap;
            if ((findData.FindOptions & FindOptions.UseRegularExpressions) == FindOptions.UseRegularExpressions)
                GetRegularExpressionMatches(findOptions, findData.SearchString, string.Empty, -1);
            var flag = (findOptions & FindOptions.WholeWord) == FindOptions.WholeWord;
            wraparound |= (findData.FindOptions & FindOptions.Wrap) == FindOptions.Wrap;
            var snapshotToSearch = findData.TextSnapshotToSearch;
            foreach (var tuple in FindAllForReplace(new SnapshotPoint(snapshotToSearch, startIndex), new SnapshotSpan(snapshotToSearch, Span.FromBounds(0, snapshotToSearch.Length)), findData.SearchString, null, findOptions & ~FindOptions.WholeWord))
            {
                if (!flag || LegacyMatchesAWholeWord(tuple.Item1, findData))
                    return tuple.Item1;
            }
            return new SnapshotSpan?();
        }

        public Collection<SnapshotSpan> FindAll(FindData findData)
        {
            if (string.IsNullOrEmpty(findData.SearchString))
                throw new ArgumentException("Search pattern can't be empty or null", nameof(findData));
            var findOptions = findData.FindOptions;
            var flag = (findOptions & FindOptions.WholeWord) == FindOptions.WholeWord;
            var snapshotToSearch = findData.TextSnapshotToSearch;
            var searchRange = new SnapshotSpan(snapshotToSearch, Span.FromBounds(0, snapshotToSearch.Length));
            var collection = new Collection<SnapshotSpan>();
            foreach (var result in FindAll(searchRange, findData.SearchString, findOptions & ~FindOptions.WholeWord))
            {
                if (!flag || LegacyMatchesAWholeWord(result, findData))
                    collection.Add(result);
            }
            return collection;
        }

        public SnapshotSpan? Find(SnapshotPoint startingPosition, string searchPattern, FindOptions options)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            return Find(startingPosition, new SnapshotSpan(startingPosition.Snapshot, Span.FromBounds(0, startingPosition.Snapshot.Length)), searchPattern, options);
        }

        public SnapshotSpan? Find(SnapshotSpan searchRange, SnapshotPoint startingPosition, string searchPattern, FindOptions options)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            if (searchRange.Snapshot != startingPosition.Snapshot)
                throw new ArgumentException("The search range and search starting position must belong to the same snapshot.");
            if (!ContainedBySpan(searchRange, startingPosition))
                throw new ArgumentException("The search start point must be contained by the search range.");
            return Find(startingPosition, searchRange, searchPattern, options);
        }

        public SnapshotSpan? FindForReplace(SnapshotPoint startingPosition, string searchPattern, string replacePattern, FindOptions options, out string expandedReplacePattern)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            if (replacePattern == null)
                throw new ArgumentNullException("Replace pattern can't be null.", nameof(replacePattern));
            return FindForReplace(startingPosition, new SnapshotSpan(startingPosition.Snapshot, Span.FromBounds(0, startingPosition.Snapshot.Length)), searchPattern, replacePattern, options, out expandedReplacePattern);
        }

        public SnapshotSpan? FindForReplace(SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options, out string expandedReplacePattern)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            if (replacePattern == null)
                throw new ArgumentNullException("Replace pattern can't be null.", nameof(replacePattern));
            return FindForReplace((options & FindOptions.SearchReverse) != FindOptions.SearchReverse ? searchRange.Start : searchRange.End, searchRange, searchPattern, replacePattern, options, out expandedReplacePattern);
        }

        public IEnumerable<SnapshotSpan> FindAll(SnapshotSpan searchRange, string searchPattern, FindOptions options)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            if (searchRange.Length == 0)
                return new SnapshotSpan[0];
            return FindAllForReplace(searchRange.Start, searchRange, searchPattern, null, options).Select(r => r.Item1);
        }

        public IEnumerable<SnapshotSpan> FindAll(SnapshotSpan searchRange, SnapshotPoint startingPosition, string searchPattern, FindOptions options)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Pattern can't be empty or null", nameof(searchPattern));
            if (searchRange.Length == 0)
                return new SnapshotSpan[0];
            if (searchRange.Snapshot != startingPosition.Snapshot)
                throw new ArgumentException("searchRange and startingPosition parameters must belong to the same snapshot.");
            if (!ContainedBySpan(searchRange, startingPosition))
                throw new InvalidOperationException("Can't perform a search when the startingPosition is not contained by the searchRange.");
            return FindAllForReplace(startingPosition, searchRange, searchPattern, null, options).Select(r => r.Item1);
        }

        public IEnumerable<Tuple<SnapshotSpan, string>> FindAllForReplace(SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options)
        {
            if (string.IsNullOrEmpty(searchPattern))
                throw new ArgumentException("Search pattern can't be null or empty.", nameof(searchPattern));
            if (replacePattern == null)
                throw new ArgumentNullException("Replace pattern can't be null.", nameof(replacePattern));
            return FindAllForReplace(searchRange.Start, searchRange, searchPattern, replacePattern, options);
        }

        private static SnapshotSpan? Find(SnapshotPoint startPosition, SnapshotSpan searchRange, string searchPattern, FindOptions options)
        {
            return FindAllForReplace(startPosition, searchRange, searchPattern, null, options).FirstOrDefault()?.Item1;
        }

        private static SnapshotSpan? FindForReplace(SnapshotPoint startPosition, SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options, out string expandedReplacePattern)
        {
            expandedReplacePattern = string.Empty;
            var tuple = FindAllForReplace(startPosition, searchRange, searchPattern, replacePattern, options).FirstOrDefault();
            if (tuple == null)
                return new SnapshotSpan?();
            expandedReplacePattern = tuple.Item2;
            return tuple.Item1;
        }

        private static IEnumerable<Tuple<SnapshotSpan, string>> FindAllForReplace(SnapshotPoint startPosition, SnapshotSpan searchRange, string searchPattern, string replacePattern, FindOptions options)
        {
            var num = (options & FindOptions.Multiline) == FindOptions.Multiline ? 1 : 0;
            var wholeWord = (options & FindOptions.WholeWord) == FindOptions.WholeWord;
            foreach (var tuple in num == 0 ? FindSingleLine(startPosition, searchRange, options, searchPattern, replacePattern) : FindMultiline(startPosition, searchRange, options, searchPattern, replacePattern))
            {
                if (!wholeWord || IsWholeWord(tuple.Item1))
                    yield return tuple;
            }
        }

        private static IEnumerable<Tuple<SnapshotSpan, string>> FindMultiline(SnapshotPoint startPosition, SnapshotSpan searchRange, FindOptions options, string searchPattern, string replacePattern = null)
        {
            var rangeText = searchRange.GetText();
            var wrap = (options & FindOptions.Wrap) == FindOptions.Wrap;
            foreach (var tuple in FindInString(searchRange.Start, startPosition - searchRange.Start, rangeText, options, searchPattern, replacePattern))
                yield return tuple;
            if (wrap)
            {
                var reverse = (options & FindOptions.SearchReverse) == FindOptions.SearchReverse;
                foreach (var tuple in FindInString(searchRange.Start, reverse ? rangeText.Length : 0, rangeText, options, searchPattern, replacePattern))
                {
                    if (reverse)
                    {
                        if (tuple.Item1.End <= startPosition)
                            break;
                    }
                    else if (tuple.Item1.Start >= startPosition)
                        break;
                    yield return tuple;
                }
            }
        }

        private static IEnumerable<Tuple<SnapshotSpan, string>> FindSingleLine(SnapshotPoint startPosition, SnapshotSpan searchRange, FindOptions options, string searchPattern, string replacePattern = null)
        {
            var reverse = (options & FindOptions.SearchReverse) == FindOptions.SearchReverse;
            var flag = (options & FindOptions.UseRegularExpressions) == FindOptions.UseRegularExpressions;
            var wrap = (options & FindOptions.Wrap) == FindOptions.Wrap;
            var containingLine = startPosition.GetContainingLine();
            var lineIncrement = reverse ? -1 : 1;
            var startLineNumber = containingLine.LineNumber;
            var snapshotPoint = searchRange.Start;
            var searchLineStart = snapshotPoint.GetContainingLine().LineNumber;
            snapshotPoint = searchRange.End;
            var searchLineEnd = snapshotPoint.GetContainingLine().LineNumber;
            var firstLineSpan = containingLine.ExtentIncludingLineBreak.Intersection(searchRange).Value;
            if (firstLineSpan.Length > 0 | flag && (!(flag & reverse) || startPosition == 0 ? 0 : (containingLine.Start == startPosition ? 1 : 0)) == 0)
            {
                foreach (var tuple in FindInString(firstLineSpan.Start, startPosition - firstLineSpan.Start, firstLineSpan.GetText(), options, searchPattern, replacePattern))
                    yield return tuple;
            }
            var i = startLineNumber + lineIncrement;
            while (i >= searchLineStart && i <= searchLineEnd)
            {
                foreach (var tuple in FindInLine(i, searchRange, options, searchPattern, replacePattern))
                    yield return tuple;
                i += lineIncrement;
            }
            if (wrap)
            {
                i = reverse ? searchLineEnd : searchLineStart;
                while (i != startLineNumber)
                {
                    foreach (var tuple in FindInLine(i, searchRange, options, searchPattern, replacePattern))
                        yield return tuple;
                    i += lineIncrement;
                }
                foreach (var tuple in FindInString(firstLineSpan.Start, reverse ? firstLineSpan.End - firstLineSpan.Start : 0, firstLineSpan.GetText(), options, searchPattern, replacePattern))
                {
                    if (reverse)
                    {
                        snapshotPoint = tuple.Item1.End;
                        if (snapshotPoint.Position <= startPosition.Position)
                            break;
                    }
                    else if (tuple.Item1.Start >= startPosition)
                        break;
                    yield return tuple;
                }
            }
        }

        private static IEnumerable<Tuple<SnapshotSpan, string>> FindInLine(int lineNumber, SnapshotSpan searchRange, FindOptions options, string searchPattern, string replacePattern)
        {
            var flag = (options & FindOptions.SearchReverse) == FindOptions.SearchReverse;
            var nullable = searchRange.Intersection(searchRange.Snapshot.GetLineFromLineNumber(lineNumber).ExtentIncludingLineBreak);
            if (nullable.HasValue)
            {
                var snapshotSpan = nullable.Value;
                var start1 = snapshotSpan.Start;
                int searchStartIndex;
                if (!flag)
                {
                    searchStartIndex = 0;
                }
                else
                {
                    snapshotSpan = nullable.Value;
                    var end = snapshotSpan.End;
                    snapshotSpan = nullable.Value;
                    var start2 = snapshotSpan.Start;
                    searchStartIndex = end - start2;
                }
                snapshotSpan = nullable.Value;
                var text = snapshotSpan.GetText();
                var num = (int)options;
                var searchTerm = searchPattern;
                var replaceTerm = replacePattern;
                foreach (var tuple in FindInString(start1, searchStartIndex, text, (FindOptions)num, searchTerm, replaceTerm))
                    yield return tuple;
            }
        }

        private static bool IsWholeWord(SnapshotSpan result)
        {
            if (result.Length < 1)
                return false;
            return UnicodeWordExtent.IsWholeWord(result);
        }

        private static bool ContainedBySpan(SnapshotSpan searchRange, SnapshotPoint startingPosition)
        {
            if (!searchRange.Contains(startingPosition))
                return searchRange.End == startingPosition;
            return true;
        }

        private static StringComparison GetStringComparison(FindOptions findOptions)
        {
            var flag = (findOptions & FindOptions.MatchCase) == FindOptions.MatchCase;
            if ((findOptions & FindOptions.OrdinalComparison) != FindOptions.OrdinalComparison)
                return flag ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            return flag ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        private static MatchCollection GetRegularExpressionMatches(FindOptions options, string searchTerm, string toSearch, int startingIndex = -1)
        {
            var regexOptions = GetRegexOptions(options);
            try
            {
                var cachedRegex = GetOrCreateCachedRegex(regexOptions, searchTerm);
                if (startingIndex == -1)
                    return cachedRegex.Matches(toSearch);
                return cachedRegex.Matches(toSearch, startingIndex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid regular expression", ex);
            }
        }

        private static Regex GetOrCreateCachedRegex(RegexOptions options, string searchTerm)
        {
            ScavengeRegexCache();
            var regexKey = GetRegexKey(options, searchTerm);
            var regex = (Regex)null;
            try
            {
                RegexCacheLock.EnterReadLock();
                if (CachedRegexEngines.TryGetValue(regexKey, out var weakReference))
                {
                    if (weakReference.IsAlive)
                        regex = weakReference.Target as Regex;
                }
            }
            finally
            {
                RegexCacheLock.ExitReadLock();
            }
            if (regex == null)
            {
                regex = new Regex(searchTerm, options);
                try
                {
                    RegexCacheLock.EnterWriteLock();
                    CachedRegexEngines[regexKey] = new WeakReference(regex);
                }
                finally
                {
                    RegexCacheLock.ExitWriteLock();
                }
            }
            return regex;
        }

        private static void ScavengeRegexCache()
        {
            if (10 != CachedRegexEngines.Count)
                return;
            try
            {
                RegexCacheLock.EnterWriteLock();
                var strArray = new string[10];
                var num = 0;
                foreach (var cachedRegexEngine in CachedRegexEngines)
                {
                    if (!cachedRegexEngine.Value.IsAlive)
                        strArray[num++] = cachedRegexEngine.Key;
                }
                for (var index = 0; index < num; ++index)
                    CachedRegexEngines.Remove(strArray[index]);
            }
            finally
            {
                RegexCacheLock.ExitWriteLock();
            }
        }

        private static string GetRegexKey(RegexOptions options, string searchTerm)
        {
            var str = (options & RegexOptions.CultureInvariant) == RegexOptions.None ? CultureInfo.CurrentCulture.ToString() : CultureInfo.InvariantCulture.ToString();
            return ((int)options).ToString(NumberFormatInfo.InvariantInfo) + ":" + str + ":" + searchTerm;
        }

        private static RegexOptions GetRegexOptions(FindOptions options)
        {
            var regexOptions = RegexOptions.None;
            if ((options & FindOptions.Multiline) == FindOptions.Multiline)
                regexOptions |= RegexOptions.Multiline;
            if ((options & FindOptions.SingleLine) == FindOptions.SingleLine)
                regexOptions |= RegexOptions.Singleline;
            if ((options & FindOptions.MatchCase) != FindOptions.MatchCase)
                regexOptions |= RegexOptions.IgnoreCase;
            if ((options & FindOptions.SearchReverse) == FindOptions.SearchReverse)
                regexOptions |= RegexOptions.RightToLeft;
            return regexOptions;
        }

        private bool LegacyMatchesAWholeWord(SnapshotSpan result, FindData findData)
        {
            var structureNavigator = findData.TextStructureNavigator ?? _navigatorSelectorService.GetTextStructureNavigator(findData.TextSnapshotToSearch.TextBuffer);
            var extentOfWord1 = structureNavigator.GetExtentOfWord(result.Start);
            var extentOfWord2 = structureNavigator.GetExtentOfWord(result.Length > 0 ? result.End - 1 : result.End);
            if (result.Start == extentOfWord1.Span.Start)
                return result.End == extentOfWord2.Span.End;
            return false;
        }

        private static IEnumerable<Tuple<SnapshotSpan, string>> FindInString(SnapshotPoint snapshotOffset, int searchStartIndex, string textData, FindOptions options, string searchTerm, string replaceTerm = null)
        {
            var reverse = (options & FindOptions.SearchReverse) == FindOptions.SearchReverse;
            if ((options & FindOptions.UseRegularExpressions) == FindOptions.UseRegularExpressions)
            {
                foreach (Match regularExpressionMatch in GetRegularExpressionMatches(options, searchTerm, textData, searchStartIndex))
                    yield return Tuple.Create(new SnapshotSpan(snapshotOffset.Snapshot, new Span(snapshotOffset + regularExpressionMatch.Index, regularExpressionMatch.Length)), replaceTerm != null ? regularExpressionMatch.Result(replaceTerm) : null);
            }
            else
            {
                while (reverse && searchStartIndex > 0 || !reverse && searchStartIndex < textData.Length)
                {
                    var num = reverse ? textData.LastIndexOf(searchTerm, searchStartIndex - 1, GetStringComparison(options)) : textData.IndexOf(searchTerm, searchStartIndex, GetStringComparison(options));
                    if (num == -1)
                        break;
                    var potentialResult = new SnapshotSpan(snapshotOffset.Snapshot, snapshotOffset + num, searchTerm.Length);
                    yield return Tuple.Create(potentialResult, replaceTerm);
                    searchStartIndex = (reverse ? potentialResult.End.Position - 1 : potentialResult.Start.Position + 1) - snapshotOffset.Position;
                }
            }
        }
    }
}