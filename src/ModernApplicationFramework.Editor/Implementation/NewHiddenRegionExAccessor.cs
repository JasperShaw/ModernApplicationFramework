using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.TextManager;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class NewHiddenRegionExAccessor : INewHiddenRegion
    {
        private static readonly int _sizeofUint = Marshal.SizeOf(typeof(uint));
        private readonly NewHiddenRegionEx _newHiddenRegion;

        internal NewHiddenRegionExAccessor(NewHiddenRegionEx newHiddenRegion)
        {
            _newHiddenRegion = newHiddenRegion;
        }

        public int Type => _newHiddenRegion.iType;

        public uint Behavior => _newHiddenRegion.dwBehavior;

        public uint State => _newHiddenRegion.dwState;

        public TextSpan HiddenText => _newHiddenRegion.tsHiddenText;

        public string Banner => _newHiddenRegion.pszBanner;

        public uint ClientData => _newHiddenRegion.dwClient;

        public uint Length
        {
            get
            {
                if (!(_newHiddenRegion.pBannerAttr != IntPtr.Zero))
                    return 0;
                return _newHiddenRegion.dwLength;
            }
        }

        public uint[] BannerAttr
        {
            get
            {
                uint[] numArray = null;
                var length = (int)Length;
                if (length > 0)
                {
                    numArray = new uint[length];
                    for (var index = 0; index < length; ++index)
                        numArray[index] = (uint)Marshal.ReadInt32((IntPtr)((int)_newHiddenRegion.pBannerAttr + _sizeofUint * index));
                }
                return numArray;
            }
        }
    }
}