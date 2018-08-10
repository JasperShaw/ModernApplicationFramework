using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextAndAdornmentCollection : IList<ISequenceElement>
    {
        ITextAndAdornmentSequencer Sequencer { get; }
    }
}