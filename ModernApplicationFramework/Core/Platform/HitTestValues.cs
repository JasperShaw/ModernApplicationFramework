namespace ModernApplicationFramework.Core.Platform
{
    internal enum HitTestValues
    {
        Hterror = -2,
        Httransparent = -1,
        Htnowhere = 0,
        Htclient = 1,
        Htcaption = 2,
        Htsysmenu = 3,
        Htgrowbox = 4,
        Htsize = Htgrowbox,
        Htmenu = 5,
        Hthscroll = 6,
        Htvscroll = 7,
        Htminbutton = 8,
        Htmaxbutton = 9,
        Htleft = 10,
        Htright = 11,
        Httop = 12,
        Httopleft = 13,
        Httopright = 14,
        Htbottom = 15,
        Htbottomleft = 16,
        Htbottomright = 17,
        Htborder = 18,
        Htreduce = Htminbutton,
        Htzoom = Htmaxbutton,
        Htsizefirst = Htleft,
        Htsizelast = Htbottomright,
        Htobject = 19,
        Htclose = 20,
        Hthelp = 21
    }
}