using ModernApplicationFramework.Core.InfoBarUtilities;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    public class UIntCookieTraits : CookieTraits<uint>
    {
        public static UIntCookieTraits Default = new UIntCookieTraits();

        public override uint UniqueCookies => (uint) ((int) MaxCookie - (int) MinCookie + 1);

        public UIntCookieTraits()
            : this(1U, uint.MaxValue, 0U)
        {
        }

        public UIntCookieTraits(uint min, uint max, uint invalid)
            : base(min, max, invalid)
        {
        }

        public override uint IncrementValue(uint current)
        {
            return checked(current + 1U);
        }
    }
}