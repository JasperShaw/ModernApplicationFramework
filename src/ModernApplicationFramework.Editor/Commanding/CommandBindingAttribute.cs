using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Commanding
{
    public class CommandBindingAttribute : MultipleBaseMetadataAttribute
    {
        public string CommandSet { get; }

        public uint CommandId { get; }

        public string CommandArgsType { get; }

        public CommandBindingAttribute(string commandSetGuid, uint commandId, Type commandArgsType)
        {
            var str = commandSetGuid;
            CommandSet = str ?? throw new ArgumentNullException(nameof(commandSetGuid));
            CommandId = commandId;
            if (commandArgsType == null)
                throw new ArgumentNullException(nameof(commandArgsType));
            CommandArgsType = commandArgsType.AssemblyQualifiedName;
        }
    }
}
