using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Settings.EditorAssociation;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.EditorBase.Settings.EditorAssociation
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(INewFileEditorAssociationSettings))]
    internal class NewFileEditorAssociationSettings : EditorFileAssociationSettings, INewFileEditorAssociationSettings
    {
        [Export]
        public static ISettingsCategory NewFileEditorAssociationCategory =
            new SettingsCategory(Guids.NewFileEditorAssociationId, SettingsCategoryType.Normal,
                "Editors_NewFileAssociations", EditorSettingsCategory.EditorCategory);


        public override ISettingsCategory Category => NewFileEditorAssociationCategory;

        public override string Name => "Editors_NewFileAssociations";

        [ImportingConstructor]
        public NewFileEditorAssociationSettings([ImportMany] IEditor[] editors, ISettingsManager settingsManager, IFileDefinitionManager fileDefinitionManager) 
            : base(editors, fileDefinitionManager, settingsManager)
        {
        }


        public override IEditor GetAssociatedEditor(ISupportedFileDefinition association)
        {
            var id = Guid.Empty;
            foreach (var cachedAssociation in CachedAssociations)
            {
                foreach (var definition in cachedAssociation.Value.CreateWithDefaultExtension)
                {
                    if (!definition.Extension.Equals(association.FileExtension))
                        continue;
                    id = new Guid(cachedAssociation.Key);
                    break;
                }
            }
            return id == Guid.Empty ? null : Editors.FirstOrDefault(x => x.EditorId == id);
        }

        protected override EditorFileAssociation CreateAssociation(IEditor editor, IEnumerable<string> list)
        {
            var association = new EditorFileAssociation(editor.EditorId.ToString("B"), editor.Name);
            association.AddRange(list, AddOption.NewFile);
            return association;
        }

        protected override void RemoveAssociations(IReadOnlyCollection<string> associations)
        {
            if (associations == null || !associations.Any())
                return;
            var cache = CachedAssociations.ToList();

            foreach (var extension in associations.ToList())
            {
                foreach (var cachedAssociation in cache)
                {
                    EditorSupportedFileDefinition def = null;
                    foreach (var definition in cachedAssociation.Value.CreateWithDefaultExtension)
                    {
                        if (!definition.Extension.Equals(extension))
                            continue;
                        def = definition;
                    }
                    if (def != null)
                        cachedAssociation.Value.CreateWithDefaultExtension.Remove(def);
                }
            }
        }

        protected override IEnumerable<string> GetAssociations(IEditor editor)
        {
            return (from valuePair in CachedAssociations
                where valuePair.Key == editor.EditorId.ToString("B")
                from definition in valuePair.Value.CreateWithDefaultExtension
                select definition.Extension).ToList();
        }
    }
}