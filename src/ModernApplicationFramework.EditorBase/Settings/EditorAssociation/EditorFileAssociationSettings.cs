using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Settings.EditorAssociation;
using ModernApplicationFramework.Settings.SettingDataModel;

namespace ModernApplicationFramework.EditorBase.Settings.EditorAssociation
{
    internal abstract class EditorFileAssociationSettings : SettingsDataModel, IEditorFileAssociationSettings
    {
        protected readonly IEditor[] Editors;
        protected readonly IFileDefinitionManager FileDefinitionManager;
        private IDictionary<string, EditorFileAssociation> _cachedAssociations;
        private readonly object _cachedInfoLock = new object();


        protected IDictionary<string, EditorFileAssociation> CachedAssociations
        {
            get
            {
                if (_cachedAssociations != null)
                    return _cachedAssociations;
                lock (_cachedInfoLock)
                {
                    _cachedAssociations = new Dictionary<string, EditorFileAssociation>();
                    var layouts = GetStoredAssociations();
                    var source = layouts.Select(association =>
                        new KeyValuePair<string, EditorFileAssociation>(association.Id, association)).ToList();
                    foreach (var pair in source)
                    {
                        if (_cachedAssociations.ContainsKey(pair.Key))
                            continue;
                        _cachedAssociations.Add(pair);
                    }
                }
                return _cachedAssociations;
            }
        }

        protected EditorFileAssociationSettings(IEditor[] editors, IFileDefinitionManager fileDefinitionManager)
        {
            SettingsChanged += EditorFileAssociationSettings_SettingsChanged;
            Editors = editors;
            FileDefinitionManager = fileDefinitionManager;
        }

        public abstract IEditor GetAssociatedEditor(ISupportedFileDefinition association);

        public void CreateAssociation(IEditor editor, ISupportedFileDefinition association)
        {
            CreateAssociations(editor, new List<ISupportedFileDefinition> { association });
        }

        public void CreateAssociations(IEditor editor, IEnumerable<ISupportedFileDefinition> associations, bool ignoreDuplicateCheck = false)
        {
            var newAssociations = associations.Select(x => x.FileExtension).ToList();
            var oldAssociations = GetAssociations(editor);

            var fullList = newAssociations.ToList();
            fullList.AddRange(oldAssociations.ToList());

            var keyValuePair = CachedAssociations.FirstOrDefault(k => k.Key.Equals(editor.EditorId.ToString("B")));
            var flag = !string.IsNullOrEmpty(keyValuePair.Key);

            if (!ignoreDuplicateCheck)
                RemoveAssociations(newAssociations);

            var editorFileAssociation = CreateAssociation(editor, fullList);

            if (!flag)
            {
                InsertSettingsModel(editorFileAssociation, true);
            }
            else
            {
                RemoveAllModels();
                var list = CachedAssociations;

                list.Remove(keyValuePair);
                list.Add(new KeyValuePair<string, EditorFileAssociation>(editor.EditorId.ToString("B"),
                    editorFileAssociation));
                foreach (var valuePair in list)
                    InsertSettingsModel(valuePair.Value, true);
            }
            OnSettingsChanged();
        }

        protected abstract EditorFileAssociation CreateAssociation(IEditor editor, IEnumerable<string> list);

        public void CreateDefaultAssociations(IEditor editor)
        {
            var definitions =
                FileDefinitionManager.SupportedFileDefinitions.Where(x => x.DefaultEditor.Equals(editor.EditorId));

            var l = new List<ISupportedFileDefinition>();
            foreach (var fileDefinition in definitions.ToList())
            {
                if (GetAssociatedEditor(fileDefinition) == null)
                    l.Add(fileDefinition);
            }

            CreateAssociations(editor, l, true);
        }

        public override void LoadOrCreate()
        {
            foreach (var editor in Editors)
                CreateDefaultAssociations(editor);
        }

        public override void StoreSettings()
        {
        }

        private async void EditorFileAssociationSettings_SettingsChanged(object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Yield();
            lock (_cachedInfoLock)
            {
                _cachedAssociations = null;
            }
        }

        private IEnumerable<EditorFileAssociation> GetStoredAssociations()
        {
            GetAllDataModel(out ICollection<EditorFileAssociation> models);
            return models;
        }

        protected abstract void RemoveAssociations(IReadOnlyCollection<string> associations);

        protected abstract IEnumerable<string> GetAssociations(IEditor editor);
    }
}
