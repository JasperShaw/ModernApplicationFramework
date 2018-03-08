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
        private IList<KeyValuePair<string, EditorFileAssociation>> _cachedAssociations;
        private readonly object _cachedInfoLock = new object();


        protected IList<KeyValuePair<string, EditorFileAssociation>> CachedAssociations
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
            var newAssociations = associations.Select(x => x.FileExtension);
            var oldAssociations = GetAssociations(editor);

            var fullList = newAssociations.ToList();
            fullList.AddRange(oldAssociations.ToList());

            var keyValuePair = CachedAssociations.FirstOrDefault(k => k.Key.Equals(editor.EditorId.ToString("B")));
            var flag = !string.IsNullOrEmpty(keyValuePair.Key);

            if (!ignoreDuplicateCheck)
                RemoveAssociations(newAssociations);

            var editorFileAssociation = CreateAssociation(editor, fullList);

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

        protected abstract EditorFileAssociation CreateAssociation(IEditor editor, IEnumerable<string> list);

        public void CreateDefaultAssociations(IEditor editor)
        {
            var definitions =
                FileDefinitionManager.SupportedFileDefinitions.Where(x => x.DefaultEditor == editor.EditorId)
                    .Where(x => GetAssociatedEditor(x) == null);
            CreateAssociations(editor, definitions, true);
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
                _cachedAssociations = null;
        }

        private IEnumerable<EditorFileAssociation> GetStoredAssociations()
        {
            GetAllDataModel(out ICollection<EditorFileAssociation> models);
            return models;
        }

        protected abstract void RemoveAssociations(IEnumerable<string> associations);

        protected abstract IEnumerable<string> GetAssociations(IEditor editor);
    }
}
