namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class NewHiddenRegionAccessor : INewHiddenRegion
    {
        private readonly NewHiddenRegion _newHiddenRegion;

        internal NewHiddenRegionAccessor(NewHiddenRegion newHiddenRegion)
        {
            _newHiddenRegion = newHiddenRegion;
        }

        public int Type => _newHiddenRegion.iType;

        public uint Behavior => _newHiddenRegion.dwBehavior;

        public uint State => _newHiddenRegion.dwState;

        public TextSpan HiddenText => _newHiddenRegion.tsHiddenText;

        public string Banner => _newHiddenRegion.pszBanner;

        public uint ClientData => _newHiddenRegion.dwClient;

        public uint Length => 0;

        public uint[] BannerAttr => null;
    }
}