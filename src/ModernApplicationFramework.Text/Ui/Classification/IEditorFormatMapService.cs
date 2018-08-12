using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IEditorFormatMapService
    {
        List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> Formats { get; }

        IEditorFormatMap GetEditorFormatMap(ITextView view);

        IEditorFormatMap GetEditorFormatMap(string category);
    }
}