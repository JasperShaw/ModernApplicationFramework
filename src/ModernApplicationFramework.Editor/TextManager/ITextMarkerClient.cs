using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.TextManager
{
    internal interface ITextMarkerClient
    {
        void MarkerInvalidated();

        int GetTipText(ITextMarker pMarker, string[] pbstrText);

        void OnBufferSave(string pszFileName);

        void OnBeforeBufferClose();

        int GetMarkerCommandInfo(ITextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf);

        int ExecMarkerCommand(ITextMarker pMarker, [In] int iItem);

        void OnAfterSpanReload();

        int OnAfterMarkerChange(ITextMarker pMarker);
    }
}