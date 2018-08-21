using System;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Input.Command
{
    public abstract class CommandDefinitionCommand : AbstractCommandWrapper, ICommandDefinitionCommand
    {
        public event EventHandler Executed;

        public event EventHandler CommandChanged;

        private CommandStatus _status;

        public virtual CommandStatus Status => _status;

        public bool Enabled
        {
            get => (uint)(_status & CommandStatus.Enabled) > 0U;
            set => SetStatus(CommandStatus.Enabled, value);
        }

        public bool Visible
        {
            get => (_status & CommandStatus.Invisible) == 0;
            set => SetStatus(CommandStatus.Invisible, !value);
        }

        public bool Supported
        {
            get => (uint)(_status & CommandStatus.Supported) > 0U;
            set => SetStatus(CommandStatus.Supported, value);
        }

        public bool Checked
        {
            get => (uint)(_status & CommandStatus.Checked) > 0U;
            set => SetStatus(CommandStatus.Checked, value);
        }

        protected CommandDefinitionCommand()
        {
            WrappedCommand = new Command(OnExecuteInternal, OnCanExecuteInternal);
            _status = CommandStatus.Enabled | CommandStatus.Supported;
        }

        protected CommandDefinitionCommand(object args)
        {
            WrappedCommand = new Command(o => OnExecuteInternal(args), o => OnCanExecuteInternal(args));
        }

        protected abstract bool OnCanExecute(object parameter);

        protected abstract void OnExecute(object parameter);

        private void OnExecuteInternal(object parameter)
        {
            OnExecute(parameter);
            OnExecuted();
        }

        private bool OnCanExecuteInternal(object parameter)
        {
            Enabled = OnCanExecute(parameter);
            return Enabled;
        }

        private void SetStatus(CommandStatus mask, bool value)
        {
            var status = _status;
            var num = !value ? status & ~mask : status | mask;
            if (num == _status)
                return;
            _status = num;
            CommandChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExecuted()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Flags]
    public enum CommandStatus
    {
        Supported = 1,
        Enabled = 2,
        Checked = 4,
        Invisible = 16,
        HideOnCtxMenu = 32
    }
}