using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.EditorBase.Settings.FontsAndColors
{
    [Export(typeof(FontAndColorInformation))]
    class FontAndColorInformation : IPartImportsSatisfiedNotification
    {
        private List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> _applicableEditorFormatDefinitions;

        private Dictionary<string, int> _itemIndexMap = new Dictionary<string, int>();

        [Import]
        private IGuardedOperations _guardedOperations;

        [ImportMany]
        internal List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> ExportedEditorFormatDefinitions { get; set; }

        public void OnImportsSatisfied()
        {
            _applicableEditorFormatDefinitions = new List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>>(ExportedEditorFormatDefinitions.Count);

            foreach (var formatDefinition in ExportedEditorFormatDefinitions)
            {
                if (formatDefinition.Metadata.UserVisible)
                {
                    if (!_itemIndexMap.ContainsKey(formatDefinition.Metadata.Name))
                    {
                        _applicableEditorFormatDefinitions.Add(formatDefinition);
                        _itemIndexMap.Add(formatDefinition.Metadata.Name, _applicableEditorFormatDefinitions.Count - 1);
                    }
                }
            }
        }
    }
}
