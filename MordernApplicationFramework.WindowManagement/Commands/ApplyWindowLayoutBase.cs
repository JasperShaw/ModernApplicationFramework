using System;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input.Command;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.Commands
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

        public override UICommand Command { get; }

        protected abstract ILayoutManager LayoutManager { get; }

        public void SetText(string text)
        {
            _text = text;
        }

        protected ApplyWindowLayoutBase()
        {
            var command = new UICommand(ApplyLayout, CanApplyLayout);
            Command = command;
        }

        private bool CanApplyLayout()
        {
            if (Index <= LayoutManager.LayoutCount)
                return true;
            return false;
        }

        private void ApplyLayout()
        {
            LayoutManager.ApplyWindowLayout(Index - 1);
        }
    }
}