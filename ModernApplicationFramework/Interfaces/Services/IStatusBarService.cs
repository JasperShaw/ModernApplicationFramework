using System.Windows.Media;
using ModernApplicationFramework.Basics.Services;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IStatusBarService
    {
        void FreezeOutput(int fFreeze);

        int GetFreezeCount();

        bool IsFrozen();

        void Clear();

        void Progress(bool inProgress, string label, uint complete, uint total);

        void SetText(string text);

        void SetText(int index, string text);

        string GetText();

        string GetText(int index);

        void SetReadyText();

        int SetVisibility(uint dwVisibility);

        bool GetVisibility();

        void SetBackgroundColor(Color color);

        void SetBackgroundColor(AbstractStatusBarService.DefaultColors color);
    }
}