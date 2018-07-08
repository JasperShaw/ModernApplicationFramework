using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public sealed class NewFileCommandDefinition : CommandDefinition<INewFileCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }

        public override GestureScope DefaultGestureScope { get; }

        public override ImageMoniker ImageMonikerSource => Monikers.NewFile;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public override Guid Id => new Guid("{B33B7AA8-2FB6-4F80-88A2-3F97878273F3}");


        public override string Name => "NewFile";
        public override string NameUnlocalized => "New File";
        public override string Text => CommandsResources.NewFileCommandText;
        public override string ToolTip => Text;

        public NewFileCommandDefinition()
        {
            DefaultKeyGestures = new []{ new MultiKeyGesture(Key.N, ModifierKeys.Control)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}
