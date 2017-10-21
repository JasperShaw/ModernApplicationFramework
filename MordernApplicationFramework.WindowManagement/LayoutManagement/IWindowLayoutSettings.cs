namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal interface IWindowLayoutSettings
    {
        bool SkipApplyLayoutConfirmation { get; set; }

        int ManageLayoutsDialogWidth { get; set; }

        int ManageLayoutsDialogHeight { get; set; }
    }
}