using System;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal abstract class CookieTraits<T> where T : IComparable<T>
    {
        protected CookieTraits(T min, T max, T invalid)
        {
            if (min.CompareTo(max) >= 0)
                throw new ArgumentException("Range");
            if (invalid.CompareTo(min) >= 0 && invalid.CompareTo(max) <= 0)
                throw new ArgumentException("Range");
            MinCookie = min;
            MaxCookie = max;
            InvalidCookie = invalid;
        }

        public T InvalidCookie { get; }

        public T MinCookie { get; }

        public T MaxCookie { get; }

        public T GetNextCookie(T current)
        {
            if (current.CompareTo(MaxCookie) < 0 && current.CompareTo(MinCookie) >= 0)
                return IncrementValue(current);
            return MinCookie;
        }

        public abstract T IncrementValue(T current);

        public abstract uint UniqueCookies { get; }
    }
}