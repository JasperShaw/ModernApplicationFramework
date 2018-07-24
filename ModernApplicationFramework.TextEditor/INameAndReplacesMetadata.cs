﻿using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface INameAndReplacesMetadata
    {
        [DefaultValue(null)]
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> Replaces { get; }
    }
}