namespace ModernApplicationFramework.Core.InfoBarUtilities
{
    public delegate void CookieTableCallback<in TCookie, in TValue>(TCookie cookie, TValue value);
}