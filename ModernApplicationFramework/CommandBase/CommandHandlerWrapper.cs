using System;
using System.Collections.Generic;
using System.Reflection;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.CommandBase
{
    public sealed class CommandHandlerWrapper
    {
        public static CommandHandlerWrapper FromCommandHandler(Type commandHandlerInterfaceType, object commandHandler)
        {
            var updateMethod = commandHandlerInterfaceType.GetMethod("Update");
            return new CommandHandlerWrapper(commandHandler, updateMethod, null);
        }

        public static CommandHandlerWrapper FromCommandListHandler(Type commandHandlerInterfaceType, object commandListHandler)
        {
            var populateMethod = commandHandlerInterfaceType.GetMethod("Populate");
            return new CommandHandlerWrapper(commandListHandler, null, populateMethod);
        }

        private readonly object _commandHandler;
        private readonly MethodInfo _updateMethod;
        private readonly MethodInfo _populateMethod;

        private CommandHandlerWrapper(
            object commandHandler,
            MethodInfo updateMethod,
            MethodInfo populateMethod)
        {
            _commandHandler = commandHandler;
            _updateMethod = updateMethod;
            _populateMethod = populateMethod;
        }

        public void Update(Command command)
        {
            if (_updateMethod != null)
                _updateMethod.Invoke(_commandHandler, new object[] { command });
        }

        public void Populate(Command command, List<DefinitionBase> commands)
        {
            if (_populateMethod == null)
                throw new InvalidOperationException("Populate can only be called for list-type commands.");
            _populateMethod.Invoke(_commandHandler, new object[] { command, commands });
        }
    }
}
