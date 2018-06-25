using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core.InfoBarUtilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.UIContext
{
    [Export(typeof(IUiContextManager))]
    internal class UiContextManager : IUiContextManager
    {
        private readonly HashSet<uint> _uiContexts = new HashSet<uint>();
        private readonly HashSet<Guid> _uiContextGuids = new HashSet<Guid>();
        private readonly CookieTable<uint, IUiContextEvents> _notifyList = new CookieTable<uint, IUiContextEvents>(UIntCookieTraits.Default);

        public int GetUiContextCookie(Guid contextGuid, out uint cookie)
        {
            var index = _uiContextGuids.IndexOf(x => x.Equals(contextGuid));
            if (index < 0)
            {
                cookie = (uint) _uiContexts.Count;
                _uiContextGuids.Add(contextGuid);
            }
            else
                cookie = (uint) index;
            return 0;
        }

        public int IsUiContextActive(uint cookie, out bool active)
        {
            active = _uiContexts.Contains(cookie);
            return 0;
        }

        public int SetUiContext(uint cookie, bool active)
        {
            if (active)
                _uiContexts.Add(cookie);
            else
                _uiContexts.Remove(cookie);
            _notifyList.ForEach((c, observer) => observer.OnUiContextChanged(cookie, active));
            return 0;
        }

        public int AdviseContextEvents(IUiContextEvents handler, out uint cookie)
        {
            cookie = _notifyList.Insert(handler);
            return 0;
        }
    }

    public interface IUiContextManager
    {
        int GetUiContextCookie(Guid contextGuid, out uint cookie);

        int IsUiContextActive(uint cookie, out bool active);

        int SetUiContext(uint cookie, bool active);

        int AdviseContextEvents(IUiContextEvents handler, out uint cookie);
    }

    public interface IUiContextEvents
    {
        int OnUiContextChanged(uint cookie, bool active);
    }

    public class UIntCookieTraits : CookieTraits<uint>
    {
        public static UIntCookieTraits Default = new UIntCookieTraits();

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

        public override uint UniqueCookies => (uint)((int)MaxCookie - (int)MinCookie + 1);
    }
}
