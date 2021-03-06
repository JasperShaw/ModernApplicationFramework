﻿namespace ModernApplicationFramework.Editor.TextManager
{
    public interface IEnumHiddenRegions
    {
        int Reset();

        int Next(uint cEl, IHiddenRegion[] ppOut, out uint pcElFetched);

        int GetCount(out uint pcRegions);
    }
}