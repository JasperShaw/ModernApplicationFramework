﻿using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandItemDefinitionBase : CommandDefinitionBase
    {
        /// <summary>
        /// The image moniker of the command definition.
        /// </summary>
        public virtual ImageMoniker ImageMonikerSource => ImageLibrary.EmptyMoniker;

        public virtual bool Checkable => false;

        /// <summary>
        /// Options that identifies the definition as a container of a list of definitions
        /// </summary>
        public abstract bool IsList { get; }
    }
}
