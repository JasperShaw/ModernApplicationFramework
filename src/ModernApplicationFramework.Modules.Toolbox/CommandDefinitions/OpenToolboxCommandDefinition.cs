using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenToolboxCommandDefinition : CommandDefinition<IOpenToolboxCommand>
    {
        public override string NameUnlocalized => "Toolbox";
        public override string Text => ToolboxResources.ToolboxCommandName;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.ViewCommandCategory;

        public override ImageMoniker ImageMonikerSource => Monikers.Toolbox;

        public override Guid Id => new Guid("{D3CD6E1A-D2E9-4EDF-A83E-FB0B110BCA7F}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public OpenToolboxCommandDefinition()
        {
            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(new List<KeySequence>
                {
                    new KeySequence(ModifierKeys.Control, Key.W),
                    new KeySequence(Key.X)
                })
            };
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}
