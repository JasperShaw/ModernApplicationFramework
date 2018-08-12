using System;
using System.Collections.Generic;
using ModernApplicationFramework.Editor.TextManager;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class EnumHiddenRegionsAdapter : IEnumHiddenRegions
    {
        private readonly IEnumerable<HiddenRegionAdapter> _hiddenRegionAdapters;
        private IEnumerator<HiddenRegionAdapter> _hiddenRegionAdapterEnumerator;
        private uint _exposed;
        private uint _count;
        private bool _countCalculated;

        public EnumHiddenRegionsAdapter(IEnumerable<HiddenRegionAdapter> vsHiddenRegionAdapters)
        {
            _hiddenRegionAdapters = vsHiddenRegionAdapters ?? throw new ArgumentNullException(nameof(vsHiddenRegionAdapters));
            _hiddenRegionAdapterEnumerator = _hiddenRegionAdapters.GetEnumerator();
            _exposed = 0U;
            _count = 0U;
            _countCalculated = false;
        }

        public int GetCount(out uint pcRegions)
        {
            if (!_countCalculated)
            {
                _count = _exposed;
                while (_hiddenRegionAdapterEnumerator.MoveNext())
                    ++_count;
                _countCalculated = true;
                _hiddenRegionAdapterEnumerator = _hiddenRegionAdapters.GetEnumerator();
                for (uint index = 0; index < _exposed; ++index)
                    _hiddenRegionAdapterEnumerator.MoveNext();
            }
            pcRegions = _count;
            return 0;
        }

        public unsafe int Next(uint cEl, IHiddenRegion[] ppOut, out uint pcElFetched)
        {
            if (cEl == 0U || ppOut == null)
            {
                fixed (uint* numPtr = &pcElFetched)
                {
                    if ((IntPtr)numPtr != IntPtr.Zero)
                        pcElFetched = 0U;
                }
                return -2147024809;
            }
            uint num = 0;
            while (num < cEl && _hiddenRegionAdapterEnumerator.MoveNext())
            {
                ppOut[num] = _hiddenRegionAdapterEnumerator.Current;
                ++num;
                ++_exposed;
            }
            fixed (uint* numPtr = &pcElFetched)
            {
                if ((IntPtr)numPtr != IntPtr.Zero)
                    pcElFetched = num;
            }
            return (int)num != (int)cEl ? 1 : 0;
        }

        public int Reset()
        {
            _hiddenRegionAdapterEnumerator = _hiddenRegionAdapters.GetEnumerator();
            _exposed = 0U;
            return 0;
        }
    }
}