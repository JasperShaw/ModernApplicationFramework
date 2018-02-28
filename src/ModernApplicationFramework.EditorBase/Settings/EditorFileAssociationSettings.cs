using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.EditorBase.Settings
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IEditorFileAssociationSettings))]
    internal class EditorFileAssociationSettings : SettingsDataModel, IEditorFileAssociationSettings
    {
        private readonly IEditor[] _editors;
        private readonly IFileDefinitionManager _fileDefinitionManager;

        [Export]
        public static ISettingsCategory EditorFileAssociationCategory =
            new SettingsCategory(Guids.EditorFileAssociationId, SettingsCategoryType.Normal,
                "Editors_FileAssociations", EditorSettingsCategory.EditorCategory);

        private IList<KeyValuePair<string, EditorFileAssociation>> _cachedAssociations;
        private readonly object _cachedInfoLock = new object();


        public override ISettingsCategory Category => EditorFileAssociationCategory;

        public override string Name => "Editors_FileAssociations";


        private IList<KeyValuePair<string, EditorFileAssociation>> CachedAssociations
        {
            get
            {
                if (_cachedAssociations != null)
                    return _cachedAssociations;
                lock (_cachedInfoLock)
                {
                    var layouts = GetStoredAssociations();
                    var source = layouts.Select(association => new KeyValuePair<string, EditorFileAssociation>(association.Id, association)).ToList();
                    _cachedAssociations = source.ToList();
                }
                return _cachedAssociations;
            }
        }

        [ImportingConstructor]
        public EditorFileAssociationSettings([ImportMany] IEditor[] editors ,ISettingsManager settingsManager, IFileDefinitionManager fileDefinitionManager)
        {
            _editors = editors;
            _fileDefinitionManager = fileDefinitionManager;
            SettingsManager = settingsManager;
            SettingsChanged += EditorFileAssociationSettings_SettingsChanged;
        }

        public IEditor GetAssociatedEditor(ISupportedFileDefinition association)
        {
            var id = Guid.Empty;
            foreach (var cachedAssociation in CachedAssociations)
            {
                foreach (var definition in cachedAssociation.Value.SupportedFileDefinition)
                {
                    if (!definition.Extension.Equals(association.FileExtension))
                        continue;
                    id = new Guid(cachedAssociation.Key);
                    break;
                }
            }
            return id == Guid.Empty ? null : _editors.FirstOrDefault(x => x.EditorId == id);
        }

        public void CreateAssociation(IEditor editor, ISupportedFileDefinition association)
        {
            CreateAssociations(editor, new List<ISupportedFileDefinition>{association});
        }

        public void CreateAssociations(IEditor editor, IEnumerable<ISupportedFileDefinition> associations, bool ignoreDuplicateCheck = false)
        {
            var newAssociations = associations.Select(x => x.FileExtension);
            var oldAssociations = GetAssociations(editor);

            var fullList = newAssociations.ToList();
            fullList.AddRange(oldAssociations.ToList());

            var keyValuePair = CachedAssociations.FirstOrDefault(k => k.Key.Equals(editor.EditorId.ToString("B")));
            var flag = !string.IsNullOrEmpty(keyValuePair.Key);

            if (!ignoreDuplicateCheck)
                RemoveAssociations(newAssociations);

            var editorFileAssociation = new EditorFileAssociation(editor.EditorId.ToString("B"), fullList);

            if (!flag)
                InsertSettingsModel(editorFileAssociation, true);
            else
            {
                RemoveAllModels();
                var list = CachedAssociations;
                list.Remove(keyValuePair);
                list.Add(new KeyValuePair<string, EditorFileAssociation>(editor.EditorId.ToString("B"), editorFileAssociation));
                foreach (var valuePair in list)
                    InsertSettingsModel(valuePair.Value, true);
            }
            OnSettingsChanged();
        }

        public void CreateDefaultAssociations(IEditor editor)
        {
            var definitions =
                _fileDefinitionManager.SupportedFileDefinitions.Where(x => x.PreferredEditor == editor.EditorId)
                    .Where(x => GetAssociatedEditor(x) == null);
            CreateAssociations(editor, definitions, true);
        }

        public override void LoadOrCreate()
        {
            foreach (var editor in _editors)
            {
                var editorEntry = CachedAssociations.FirstOrDefault(k => k.Key.Equals(editor.EditorId.ToString("B"))).Value;
                if (editorEntry == null)
                    CreateDefaultAssociations(editor);
            }
        }

        public override void StoreSettings()
        {
        }

        private async void EditorFileAssociationSettings_SettingsChanged(object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Yield();
            lock (_cachedInfoLock)
                _cachedAssociations = null;
        }

        private IEnumerable<EditorFileAssociation> GetStoredAssociations()
        {
            GetAllDataModel(out ICollection<EditorFileAssociation> models);
            return models;
        }

        private void RemoveAssociations(IEnumerable<string> associations)
        {
            if (associations == null || !associations.Any())
                return;
            var cache = CachedAssociations.ToList();

            foreach (var extension in associations.ToList())
            {
                foreach (var cachedAssociation in cache)
                {
                    EditorSupportedFileDefinition def = null;
                    foreach (var definition in cachedAssociation.Value.SupportedFileDefinition)
                    {
                        if (!definition.Extension.Equals(extension))
                            continue;
                        def = definition;
                    }
                    if (def != null)
                        cachedAssociation.Value.SupportedFileDefinition.Remove(def);
                }   
            }
        }

        private IEnumerable<string> GetAssociations(IEditor editor)
        {
            return (from valuePair in CachedAssociations
                where valuePair.Key == editor.EditorId.ToString("B")
                from definition in valuePair.Value.SupportedFileDefinition
                select definition.Extension).ToList();
        }
    }
}
