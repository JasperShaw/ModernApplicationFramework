using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    internal class ApplyWindowLayoutCommand : CommandDefinitionCommand
    {
        public ApplyWindowLayoutCommand(int index) : base(index)
        {
            
        }

        protected override bool OnCanExecute(object parameter)
        {
            if (!(parameter is int))
                return false;
            if ((int)parameter <= LayoutManagementService.Instance?.LayoutManager.LayoutCount)
                return true;
            return false;
        }

        protected override void OnExecute(object parameter)
        {
            LayoutManagementService.Instance.LayoutManager.ApplyWindowLayout((int)parameter - 1);
        }
    }
}