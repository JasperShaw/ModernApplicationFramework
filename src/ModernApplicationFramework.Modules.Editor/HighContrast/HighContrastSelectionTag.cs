using System;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Modules.Editor.HighContrast
{
    internal class HighContrastSelectionTag : IClassificationTag
    {
        private static HighContrastSelectionTag _instance;

        public static HighContrastSelectionTag Instance
        {
            get
            {
                var instance = _instance;
                if (instance != null)
                    return instance;
                throw new InvalidOperationException("Tag is not yet initialized.");
            }
        }

        public IClassificationType ClassificationType { get; }

        private HighContrastSelectionTag(IClassificationType classificationType)
        {
            ClassificationType = classificationType ?? throw new ArgumentNullException(nameof(classificationType));
        }

        internal static void Initialize(IClassificationTypeRegistryService classificationRegistry)
        {
            if (_instance != null)
                return;
            if (classificationRegistry == null)
                throw new ArgumentNullException(nameof(classificationRegistry));
            _instance = new HighContrastSelectionTag(
                classificationRegistry.GetClassificationType("HighContrastSelection"));
        }
    }
}