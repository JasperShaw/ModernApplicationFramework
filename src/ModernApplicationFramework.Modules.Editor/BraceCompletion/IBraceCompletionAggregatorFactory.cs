using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal interface IBraceCompletionAggregatorFactory
    {
        IBraceCompletionAggregator CreateAggregator();

        IEnumerable<string> ContentTypes { get; }
    }
}