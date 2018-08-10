using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.TextEditor.Implementation;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextStructureNavigatorProvider))]
    [ContentType("any")]
    internal sealed class NaturalLanguageNavigatorProvider : ITextStructureNavigatorProvider
    {
        public ITextStructureNavigator CreateTextStructureNavigator(ITextBuffer textBuffer)
        {
            if (textBuffer == null)
                throw new ArgumentNullException(nameof(textBuffer));
            return new NaturalLanguageNavigator(textBuffer);
        }
    }
}