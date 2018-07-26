using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextAndAdornmentCollection : IList<ISequenceElement>
    {
        ITextAndAdornmentSequencer Sequencer { get; }
    }
}