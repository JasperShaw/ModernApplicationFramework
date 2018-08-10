namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class NewOutlineRegionAccessor : INewHiddenRegion
    {
        private readonly NewOutlineRegion _newOutlineRegion;

        internal NewOutlineRegionAccessor(NewOutlineRegion newOutlineRegion)
        {
            _newOutlineRegion = newOutlineRegion;
        }

        public int Type => 1;

        public uint Behavior => 1;

        public uint State => _newOutlineRegion.dwState;

        public TextSpan HiddenText => _newOutlineRegion.tsHiddenText;

        public string Banner => "...";

        public uint ClientData => 0;

        public uint Length => 0;

        public uint[] BannerAttr => null;
    }
}