using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewVerticalTabGroupCommandDefinition))]
    public sealed class NewVerticalTabGroupCommandDefinition : CommandDefinition<INewVerticalTabGroupCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("NewVerticalTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.NewVerticalTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override Uri IconSource =>
            new Uri("/ModernApplicationFramework;component/Resources/Icons/SplitScreenVertical.xaml",
                UriKind.RelativeOrAbsolute);

        public override string IconId => "SplitScreenVertical";

        public override ImageMoniker ImageMonikerSource => Monikers.SplitScreenVertical;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{5667C276-A91F-428A-86A3-7D95814B4B9F}");
    }
}