using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(IBraceCompletionDefaultProvider))]
    [BracePair('(', ')')]
    [BracePair('"', '"')]
    [BracePair('{', '}')]
    [BracePair('[', ']')]
    [ContentType("plaintext")]
    internal sealed class PlainTextDefaults : IBraceCompletionDefaultProvider
    {
    }
}
