using System;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    public abstract class ApplyWindowLayoutBase : CommandDefinition
    {
        private string _text;
        public abstract int Index { get; }

        public sealed override string IconId => null;

        public sealed override Uri IconSource => null;

        public sealed override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public sealed override string Name => string.Format(CultureInfo.CurrentUICulture,
            WindowManagement_Resources.ApplyWindowLayoutCommantDefinition_Name, new object[]
            {
                Index
            });

        public override string NameUnlocalized => string.Format(CultureInfo.CurrentUICulture,
            WindowManagement_Resources.ResourceManager.GetString("ApplyWindowLayoutCommantDefinition_Name",
                CultureInfo.InvariantCulture) ?? throw new InvalidOperationException(), new object[]
            {
                Index
            });

        public sealed override string ToolTip => Text;
        public sealed override string Text => _text;

        public void SetText(string text)
        {
            _text = text;
        }

        protected void SetCommand()
        {
            Command = new ApplyWindowLayoutCommand(Index);
        }
    }

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