using System;
using System.Globalization;
using System.IO;
using System.Text;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.EditorBase.Utilities
{
    internal static class PathUtilities
    {
        private static readonly char[] DirectorySeparators = {
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        };

        public static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException(nameof(path));
            int index1 = 0;
            while (index1 < path.Length && char.IsWhiteSpace(path[index1]))
                ++index1;
            if (index1 == path.Length)
                return string.Empty;
            int index2 = path.Length - 1;
            while (index2 >= index1 && char.IsWhiteSpace(path[index2]))
                --index2;
            int num = index2 - index1 + 1;
            using (var reusableResourceHolder = ReusableStringBuilder.AcquireDefault(260))
            {
                StringBuilder resource = reusableResourceHolder.Resource;
                bool flag1 = false;
                bool flag2 = false;
                for (int index3 = index1; index3 <= index2; ++index3)
                {
                    char c = path[index3];
                    if (c == Path.AltDirectorySeparatorChar)
                    {
                        c = Path.DirectorySeparatorChar;
                        flag2 = true;
                    }
                    if (c == Path.DirectorySeparatorChar)
                    {
                        if (flag1 && index3 > index1 + 1)
                        {
                            flag2 = true;
                            continue;
                        }
                    }
                    else if (char.IsUpper(c))
                    {
                        c = char.ToLower(c);
                        flag2 = true;
                    }
                    flag1 = c == Path.DirectorySeparatorChar;
                    resource.Append(c);
                }
                if (flag1 && resource.Length > 3)
                {
                    resource.Remove(resource.Length - 1, 1);
                    flag2 = true;
                }
                if (!flag2 && num == path.Length)
                    return path;
                return resource.ToString();
            }
        }

        public static bool IsNormalized(string path)
        {
            return path == Normalize(path);
        }

        public static string NormalizePath(this string path)
        {
            return Normalize(path);
        }

        public static bool IsNormalizedPath(this string path)
        {
            return IsNormalized(path);
        }

        public static bool IsDescendant(string parent, string child)
        {
            Validate.IsNotNullAndNotWhiteSpace(parent, "parent");
            Validate.IsNotNullAndNotWhiteSpace(child, "child");
            int length = parent.Length;
            while (IsDirectorySeparator(parent[length - 1]))
                --length;
            return child.Length >= length && string.Compare(parent, 0, child, 0, length, StringComparison.OrdinalIgnoreCase) == 0 && (child.Length <= length || child[length] == Path.DirectorySeparatorChar);
        }

        public static string GetCommonPathPrefix(string path1, string path2)
        {
            Validate.IsNotNull(path1, "path1");
            Validate.IsNotNull(path2, "path2");
            if (path1.Length == 0 || path2.Length == 0)
                return string.Empty;
            if (ArePathsEqual(path1, path2))
                return path1;
            bool flag = path1.StartsWith("\\\\");
            if (flag != path2.StartsWith("\\\\"))
                return string.Empty;
            using (ReusableResourceHolder<StringBuilder> reusableResourceHolder = ReusableStringBuilder.AcquireDefault(260))
            {
                StringBuilder resource = reusableResourceHolder.Resource;
                PathParser pathParser = new PathParser(path1);
                PathParser other = new PathParser(path2);
                while (pathParser.MoveNext() && other.MoveNext() && (pathParser.CurrentLength == other.CurrentLength && pathParser.CompareCurrentSegment(other) == 0))
                {
                    if (resource.Length == 0 & flag)
                        resource.Append("\\\\");
                    else if (resource.Length > 0)
                        resource.Append(Path.DirectorySeparatorChar);
                    resource.Append(path1, pathParser.CurrentStartIndex, pathParser.CurrentLength);
                }
                if (resource.Length == 2 && resource[1] == Path.VolumeSeparatorChar)
                    resource.Append('\\');
                return resource.ToString();
            }
        }

        public static bool ArePathsEqual(string path1, string path2)
        {
            return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            return ArePathsEqual(path, Path.GetPathRoot(path));
        }

        public static bool IsReparsePoint(string path)
        {
            Validate.IsNotNullAndNotEmpty(path, "path");
            if (Directory.Exists(path))
            {
                try
                {
                    return (uint)(File.GetAttributes(path) & FileAttributes.ReparsePoint) > 0U;
                }
                catch (FileNotFoundException)
                {
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
            return false;
        }

        public static bool ContainsReparsePoint(string path, string pathRoot = null)
        {
            Validate.IsNotNullAndNotEmpty(path, "path");
            string path1 = path;
            string str = string.Empty;
            if (pathRoot != null)
                str = GetCommonPathPrefix(path, pathRoot);
            for (; path1 != null && path1.Length > str.Length; path1 = Path.GetDirectoryName(path1))
            {
                if (IsReparsePoint(path1))
                    return true;
            }
            return false;
        }

        public static bool IsDirectorySeparator(char c)
        {
            if (c != Path.DirectorySeparatorChar)
                return c == Path.AltDirectorySeparatorChar;
            return true;
        }

        public static bool IsImplicitDirectory(string directory)
        {
            Validate.IsNotNull(directory, "directory");
            string fileName = Path.GetFileName(directory);
            if (fileName != null)
                switch (fileName.Length)
                {
                    case 1:
                        return fileName[0] == 46;
                    case 2:
                        if (fileName[1] != 46)
                            break;
                        goto case 1;
                }
            return false;
        }

        public static string SafeGetExtension(string path)
        {
            if (path == null)
                return string.Empty;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                return Path.GetExtension(path);
            char[] anyOf = {
                '.',
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
            };
            int startIndex = path.LastIndexOfAny(anyOf);
            if (startIndex >= 0 && path[startIndex] == 46)
                return path.Substring(startIndex);
            return string.Empty;
        }

        public static string GetBaseFilePath(string fullFilePath)
        {
            if (string.IsNullOrEmpty(fullFilePath))
                return string.Empty;
            if (!Path.IsPathRooted(fullFilePath))
                return string.Empty;
            return Path.GetDirectoryName(fullFilePath).NormalizePath() + Path.DirectorySeparatorChar;
        }

        public static string ReplaceEnvironmentPrefix(string s, string variableName)
        {
            return ReplaceEnvironmentPrefix(s, variableName, CaseSensitivity.Insensitive);
        }

        public static string ReplaceEnvironmentPrefix(string s, string variableName, CaseSensitivity sensitivity)
        {
            if (variableName == null)
                throw new ArgumentNullException(nameof(variableName));
            if (string.IsNullOrEmpty(s) || variableName == string.Empty)
                return s;
            string environmentVariable = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrEmpty(environmentVariable) || !s.StartsWith(environmentVariable, sensitivity == CaseSensitivity.Insensitive, CultureInfo.CurrentCulture))
                return s;
            return $"%{variableName}%{s.Substring(environmentVariable.Length)}";
        }

        private class PathParser
        {
            private int _startIndex;
            private int _length;

            private string Path { get; }

            public int CurrentStartIndex => _startIndex;

            public int CurrentLength => _length;

            public PathParser(string path)
            {
                Validate.IsNotNull(path, "path");
                Path = path;
            }

            public bool MoveNext()
            {
                _startIndex = _startIndex + _length;
                while (_startIndex < Path.Length && IsDirectorySeparator(Path[_startIndex]))
                    _startIndex = _startIndex + 1;
                if (_startIndex >= Path.Length)
                    return false;
                int num = Path.IndexOfAny(DirectorySeparators, _startIndex);
                if (num == -1)
                    num = Path.Length;
                _length = num - _startIndex;
                return true;
            }

            public int CompareCurrentSegment(PathParser other)
            {
                return string.Compare(Path, _startIndex, other.Path, other._startIndex, _length, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    public enum CaseSensitivity
    {
        Sensitive,
        Insensitive,
    }
}
