﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal static class ToolWindowContentTypes
    {
        [Export]
        [Name("Output")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition OutputContentType;
    }
}
