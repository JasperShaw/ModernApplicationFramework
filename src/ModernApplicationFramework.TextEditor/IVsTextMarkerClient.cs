using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor
{
    internal interface IVsTextMarkerClient
    {
        void MarkerInvalidated();

        int GetTipText(IVsTextMarker pMarker, string[] pbstrText);

        void OnBufferSave(string pszFileName);

        void OnBeforeBufferClose();

        int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf);

        int ExecMarkerCommand(IVsTextMarker pMarker, [In] int iItem);

        void OnAfterSpanReload();

        int OnAfterMarkerChange(IVsTextMarker pMarker);
    }
}