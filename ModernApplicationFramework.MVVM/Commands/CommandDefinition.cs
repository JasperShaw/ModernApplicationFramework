using System;

namespace ModernApplicationFramework.MVVM.Commands
{
    internal abstract class CommandDefinition : CommandDefinitionBase
    {
        public override Uri IconSource => null;

        public sealed override bool IsList => false;
    }
}