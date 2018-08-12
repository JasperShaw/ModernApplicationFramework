using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.TextAndAdornmentSequencer
{
    internal sealed class TextAndAdornmentCollection : ReadOnlyCollection<ISequenceElement>, ITextAndAdornmentCollection
    {
        public TextAndAdornmentCollection(ITextAndAdornmentSequencer sequencer, IList<ISequenceElement> elements)
            : base(elements)
        {
            Sequencer = sequencer;
        }

        public ITextAndAdornmentSequencer Sequencer { get; }
    }
}