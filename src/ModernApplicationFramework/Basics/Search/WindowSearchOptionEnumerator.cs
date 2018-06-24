using System.Collections.Generic;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search
{
    public class WindowSearchOptionEnumerator : IEnumWindowSearchOptions
    {
        private readonly IEnumerable<IWindowSearchOption> _options;
        private IEnumerator<IWindowSearchOption> _enumerator;
        private int _enumeratorPosition;

        public WindowSearchOptionEnumerator(IEnumerable<IWindowSearchOption> options)
        {
            _options = options;
            ResetEnumerator();
        }

        private WindowSearchOptionEnumerator(IEnumerable<IWindowSearchOption> options, int enumeratorPosition) : this(options)
        {
            for (int index = 0; index < enumeratorPosition; ++index)
                MoveEnumerator();
        }

        public int Next(uint celt, IWindowSearchOption[] rgelt, out uint fetched)
        {
            uint num = 0;
            while (celt-- != 0 && MoveEnumerator())
            {
                rgelt[num] = _enumerator.Current;
                ++num;
            }
            fetched = num;
            return num <= 0 ? 1 : 0;
        }


        public int Skip(uint celt)
        {
            var flag = true;
            while (celt-- > 0 & flag)
                flag = MoveEnumerator();
            return !flag ? 1 : 0;
        }

        public void Reset()
        {
            ResetEnumerator();
        }

        public void Clone(out IEnumWindowSearchOptions options)
        {
            options = new WindowSearchOptionEnumerator(_options, _enumeratorPosition);
        }

        private bool MoveEnumerator()
        {
            if (!_enumerator.MoveNext())
                return false;
            ++_enumeratorPosition;
            return true;
        }

        private void ResetEnumerator()
        {
            _enumeratorPosition = 0;
            _enumerator = _options.GetEnumerator();
        }
    }
}