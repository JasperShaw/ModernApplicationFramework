using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation.OutputClassifier
{
    public class OutputClassificationTypes
    {
        [Export]
        [Name("OutputHeading")]
        public static ClassificationTypeDefinition OutputHeadingClassificationType { get; set; }

        [Export]
        [Name("OutputError")]
        public static ClassificationTypeDefinition OutputErrorClassificationType { get; set; }

        [Export]
        [Name("OutputVerbose")]
        public static ClassificationTypeDefinition OutputVerboseClassificationType { get; set; }
    }
}
