using System;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.WindowManagement.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    public abstract class ApplyWindowLayoutBase : CommandDefinition
    {
        private string _text;
        public abstract int Index { get; }

        public sealed override CommandBarCategory Category => CommandBarCategories.WindowCategory;

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
}