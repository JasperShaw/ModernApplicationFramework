using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Operations
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