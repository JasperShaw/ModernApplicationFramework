using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Extended.Core.LayoutManagement;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(DefinitionBase))]
    [Export(typeof(SaveCurrentLayoutCommandDefinition))]
    public sealed class SaveCurrentLayoutCommandDefinition : CommandDefinition
    {
        private readonly ILayoutManager _layoutManager;
        public override string Name => Commands_Resources.SaveLayoutCommandDefinition_Name;
        public override string Text => Commands_Resources.SaveLayoutCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public SaveCurrentLayoutCommandDefinition(ILayoutManager layoutManager)
        {
            _layoutManager = layoutManager;

            var command = new MultiKeyGestureCommandWrapper(Save, CanSave);
            Command = command;
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            var result = TextInputDialog.Show(Commands_Resources.SaveLayoutCommandDefinitionMessageBox_Title,
                Commands_Resources.SaveLayoutCommandDefinitionMessageBox_Label, _layoutManager.GetUniqueLayoutName(),
                s => !string.IsNullOrEmpty(s), out string layoutName);
            if (!result)
                return;
            _layoutManager.Save(layoutName);
        }
    }
}
