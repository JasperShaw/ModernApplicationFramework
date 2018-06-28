using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveAllCommandDefinition))]
    public class SaveAllCommandDefinition : CommandDefinition<ISaveAllCommand>
    {
        public override string NameUnlocalized => "Save active file";

        public override string Text => CommandsResources.SaveAllCommandText;

        public override string ToolTip => Text;

        public override Uri IconSource => new Uri("/ModernApplicationFramework.EditorBase.CommandBar;component/Resources/Icons/SaveAll_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "SaveAllFilesIcon";
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{651EA782-BFCB-4ACA-8F98-6798C117F988}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public SaveAllCommandDefinition()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}
