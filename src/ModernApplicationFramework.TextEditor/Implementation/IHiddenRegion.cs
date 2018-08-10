namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IHiddenRegion
    {
        int GetType(out int piHiddenRegionType);

        int GetBehavior(out uint pdwBehavior);

        int GetState(out uint dwState);

        int SetState(uint dwState, uint dwUpdate);

        int GetBanner(out string pbstrBanner);

        int SetBanner(string pszBanner);

        int GetSpan(TextSpan[] pSpan);

        int SetSpan(TextSpan[] pSpan);

        int GetClientData(out uint pdwData);

        int SetClientData(uint dwData);

        int Invalidate(uint dwUpdate);

        int IsValid();

        int GetBaseBuffer(out IMafTextLines ppBuffer);
    }
}