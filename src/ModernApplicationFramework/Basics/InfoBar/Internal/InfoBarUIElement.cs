using System.Windows;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Core.InfoBarUtilities;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.InfoBar.Internal
{
    internal class InfoBarUiElement : IInfoBarUiElement
    {

        private readonly CookieTable<uint, IInfoBarUiEvents> _cookieTable = new CookieTable<uint, IInfoBarUiEvents>(UIntCookieTraits.Default);
        private InfoBarControl _infoBarControl;

        public InfoBarUiElement(InfoBarModel infoBar)
        {
            Validate.IsNotNull(infoBar, nameof(infoBar));
            ViewModel = new InfoBarViewModel(infoBar) {Owner = this};
        }

        public InfoBarViewModel ViewModel { get;}


        public int Advise(IInfoBarUiEvents eventSink, out uint cookie)
        {
            cookie = _cookieTable.Insert(eventSink);
            return 0;
        }

        public int Unadvise(uint cookie)
        {
            _cookieTable.Remove(cookie);
            return 0;
        }


        public FrameworkElement CreateControl()
        {
            _infoBarControl = new InfoBarControl {DataContext = ViewModel};
            return _infoBarControl;
        }

        public int Close()
        {
            return ForEach(NotifyClosed);
        }

        public int ForEach(CookieTableCallback<uint, IInfoBarUiEvents> handler)
        {
            _cookieTable.ForEach(handler);
            return 0;
        }

        private void NotifyClosed(uint cookie, IInfoBarUiEvents events)
        {
            events.OnClosed(this);
        }

        private class UIntCookieTraits : CookieTraits<uint>
        {
            public static readonly UIntCookieTraits Default = new UIntCookieTraits();

            private UIntCookieTraits()
                : this(1U, uint.MaxValue, 0U)
            {
            }

            private UIntCookieTraits(uint min, uint max, uint invalid)
                : base(min, max, invalid)
            {
            }

            public override uint IncrementValue(uint current)
            {
                return checked(current + 1U);
            }

            public override uint UniqueCookies => (uint)((int)MaxCookie - (int)MinCookie + 1);
        }

    }
}
