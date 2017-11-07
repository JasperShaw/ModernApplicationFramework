namespace ModernApplicationFramework.Core.InfoBarUtilities
{
    internal delegate void CookieTableCallback<in TCookie, in TValue>(TCookie cookie, TValue value);
}