using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(NewFileCommandDefinition))]
    public class OpenFileCommandDefinition : CommandDefinition<IOpenFileCommand>
    {
        public override string NameUnlocalized => "Open File";
        public override string Name => "OpenFile";
        public override string Text => CommandsResources.OpenFileCommandText;
        public override string ToolTip => Text;
        public override ImageMoniker ImageMonikerSource => Monikers.OpenFolder;
        public override CommandBarCategory Category => CommandCategories.FileCategory;
        public override Guid Id => new Guid("{47E7AF89-3733-4FBF-A3FA-E8AD5D5C693E}");

        public override ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes => new ReadOnlyCollection<GestureScopeMapping>(new[]
        {
            new GestureScopeMapping(GestureScopes.GlobalGestureScope, new MultiKeyGesture(Key.O, ModifierKeys.Control))
        });
    }
}
