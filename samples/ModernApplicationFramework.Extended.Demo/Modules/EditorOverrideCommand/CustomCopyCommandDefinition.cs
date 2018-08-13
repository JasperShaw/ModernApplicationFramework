using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces.Commands;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Extended.Demo.Modules.EditorOverrideCommand
{
    //[Export(typeof(CommandDefinitionBase))]
    //internal class CustomCopyCommandDefinition : CommandDefinition<ICustomCopyCommand>
    //{
    //    public override string NameUnlocalized => "TestCommand";
    //    public override string Text => "TestCommand";
    //    public override string ToolTip => Text;
    //    public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
    //    public override Guid Id => new Guid("{58715058-03D9-4D8E-88AC-511DD21A6E9D}");
    //    public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
    //    public override GestureScope DefaultGestureScope => null;
    //}

    internal interface ICustomCopyCommand : ICommandDefinitionCommand
    {
    }

    //[Export(typeof(ICustomCopyCommand))]
    [Export(typeof(Text.Ui.Commanding.ICommandHandler))]
    [ContentType("output")]
    [Name("Test Command")]
    internal class CustomCopyCommand : CommandDefinitionCommand, ICustomCopyCommand, Text.Ui.Commanding.ICommandHandler
    {
        public CustomCopyCommand()
        {
            
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {

        }
    }
}
