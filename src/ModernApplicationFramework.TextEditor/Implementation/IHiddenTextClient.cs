namespace ModernApplicationFramework.TextEditor.Implementation
{

    //TODO: Implement
    public interface IHiddenTextClient
    {
        void OnHiddenRegionChange(IHiddenRegion pHidReg, HiddenRegionEvent eventCode, int fBufferModifiable);

        int GetTipText(IHiddenRegion pHidReg, string[] pbstrText);

        int GetMarkerCommandInfo(IHiddenRegion pHidReg, int iItem, string[] pbstrText, uint[] pcmdf);

        int ExecMarkerCommand(IHiddenRegion pHidReg, int iItem);

        int MakeBaseSpanVisible(IHiddenRegion pHidReg, TextSpan[] pBaseSpan);

        void OnBeforeSessionEnd();
    }
}