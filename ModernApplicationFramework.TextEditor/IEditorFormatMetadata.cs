﻿using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorFormatMetadata
    {
        string Name { get; }

        [DefaultValue(false)]
        bool UserVisible { get; }

        [DefaultValue(0)]
        int Priority { get; }
    }
}