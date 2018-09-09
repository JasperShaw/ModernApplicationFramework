using System;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Differencing
{
    public interface IDifferenceTextViewModel : ITextViewModel, IPropertyOwner, IDisposable
    {
        IDifferenceViewer Viewer { get; }

        DifferenceViewType ViewType { get; }
    }
}