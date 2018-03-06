using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.EditorBase.Settings
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
        public NewFileEditorAssociationSettings([ImportMany] IEditor[] editors, ISettingsManager settingsManager, IFileDefinitionManager fileDefinitionManager) : base(editors, fileDefinitionManager)
        {
            SettingsManager = settingsManager;
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

        protected override void RemoveAssociations(IEnumerable<string> associations)
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


    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IOpenFileEditorAssociationSettings))]
    internal class OpenFileEditorAssociationSettings : EditorFileAssociationSettings, IOpenFileEditorAssociationSettings
    {
        [Export]
        public static ISettingsCategory OpenFileEditorAssociationCategory =
            new SettingsCategory(Guids.OpenFileEditorAssociationId, SettingsCategoryType.Normal,
                "Editors_OpenFileAssociations", EditorSettingsCategory.EditorCategory);


        public override ISettingsCategory Category => OpenFileEditorAssociationCategory;

        public override string Name => "Editors_OpenFileAssociations";

        [ImportingConstructor]
        public OpenFileEditorAssociationSettings([ImportMany] IEditor[] editors, ISettingsManager settingsManager, IFileDefinitionManager fileDefinitionManager) : base(editors, fileDefinitionManager)
        {
            SettingsManager = settingsManager;
        }


        public override IEditor GetAssociatedEditor(ISupportedFileDefinition association)
        {
            var id = Guid.Empty;
            foreach (var cachedAssociation in CachedAssociations)
            {
                foreach (var definition in cachedAssociation.Value.DefaultExtension)
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
            association.AddRange(list, AddOption.OpenFile);
            return association;
        }

        protected override void RemoveAssociations(IEnumerable<string> associations)
        {
            if (associations == null || !associations.Any())
                return;
            var cache = CachedAssociations.ToList();

            foreach (var extension in associations.ToList())
            {
                foreach (var cachedAssociation in cache)
                {
                    EditorSupportedFileDefinition def = null;
                    foreach (var definition in cachedAssociation.Value.DefaultExtension)
                    {
                        if (!definition.Extension.Equals(extension))
                            continue;
                        def = definition;
                    }
                    if (def != null)
                        cachedAssociation.Value.DefaultExtension.Remove(def);
                }
            }
        }

        protected override IEnumerable<string> GetAssociations(IEditor editor)
        {
            return (from valuePair in CachedAssociations
                    where valuePair.Key == editor.EditorId.ToString("B")
                    from definition in valuePair.Value.DefaultExtension
                    select definition.Extension).ToList();
        }
    }

    public interface INewFileEditorAssociationSettings : IEditorFileAssociationSettings
    {
    }

    public interface IOpenFileEditorAssociationSettings : IEditorFileAssociationSettings
    {
    }
}
