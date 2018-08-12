using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Storage;
using ModernApplicationFramework.Text.Ui.Classification;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal sealed class EditorFormatMap : IEditorFormatMap
    {
        private readonly List<string> _changedItems = new List<string>();
        private readonly IDataStorage _dataStorage;
        private readonly GuardedOperations _guardedOperations;

        private readonly Dictionary<string, ResourceDictionary> _storedResourceDictionaries =
            new Dictionary<string, ResourceDictionary>();

        private Dictionary<string, Lazy<EditorFormatDefinition, IEditorFormatMetadata>> _provisionNameMap;

        public event EventHandler<FormatItemsEventArgs> FormatMappingChanged;

        public bool IsInBatchUpdate { get; private set; }

        private List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> Formats { get; }

        public EditorFormatMap(List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> formats,
            IDataStorage dataStorage, GuardedOperations guardedOperations)
        {
            Formats = formats;
            _dataStorage = dataStorage;
            _guardedOperations = guardedOperations ?? new GuardedOperations();
        }

        public void AddProperties(string key, ResourceDictionary properties)
        {
            SetProperties(key, properties);
        }

        public void BeginBatchUpdate()
        {
            if (IsInBatchUpdate)
                throw new InvalidOperationException("BeginBatchUpdate called twice without calling EndBatchUpdate");
            IsInBatchUpdate = true;
        }

        public void EndBatchUpdate()
        {
            if (!IsInBatchUpdate)
                throw new InvalidOperationException("EndBatchUpdate called without BeginBatchUpdate being called");
            IsInBatchUpdate = false;
            SendChangedEvent();
        }

        public ResourceDictionary GetProperties(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return CreateResourceDictionaryFromProvision(key);
        }

        public void SetProperties(string key, ResourceDictionary properties)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            _storedResourceDictionaries[key] = properties;
            _changedItems.Add(key);
            SendChangedEvent();
        }

        private ResourceDictionary CreateResourceDictionaryFromProvision(string key)
        {
            if (_storedResourceDictionaries.TryGetValue(key, out var resourceDictionary1))
                return resourceDictionary1;
            var provision = FindProvision(key);
            var resourceDictionary2 =
                provision == null ? new ResourceDictionary() : provision.CreateResourceDictionary();
            if (_dataStorage != null && _dataStorage.TryGetItemValue(key, out var itemValue))
            {
                if (!itemValue.Contains("ForegroundColor"))
                {
                    resourceDictionary2.Remove("ForegroundColor");
                    resourceDictionary2.Remove("Foreground");
                }

                if (!itemValue.Contains("BackgroundColor"))
                {
                    resourceDictionary2.Remove("BackgroundColor");
                    resourceDictionary2.Remove("Background");
                }

                foreach (var key1 in itemValue.Keys)
                    resourceDictionary2[key1] = itemValue[key1];
            }

            _storedResourceDictionaries[key] = resourceDictionary2;
            return resourceDictionary2;
        }

        private EditorFormatDefinition FindProvision(string key)
        {
            if (_provisionNameMap == null)
            {
                _provisionNameMap =
                    new Dictionary<string, Lazy<EditorFormatDefinition, IEditorFormatMetadata>>(Formats.Count,
                        StringComparer.InvariantCultureIgnoreCase);
                foreach (var format in Formats)
                {
                    var name = format.Metadata.Name;
                    if (_provisionNameMap.TryGetValue(name, out var lazy))
                    {
                        if (lazy.Metadata.Priority < format.Metadata.Priority)
                            _provisionNameMap[name] = format;
                        else if (lazy.Metadata.Priority == format.Metadata.Priority)
                            _guardedOperations.HandleException(format,
                                new ImportCardinalityMismatchException(string.Format(CultureInfo.InvariantCulture,
                                    "Duplicate EditorFormatDefinition exports with identical name attributes exist. Duplicate name is {0}",
                                    name)));
                    }
                    else
                    {
                        _provisionNameMap.Add(name, format);
                    }
                }
            }

            if (_provisionNameMap.TryGetValue(key, out var provider))
                return _guardedOperations.InstantiateExtension(provider, provider);
            return null;
        }

        private void SendChangedEvent()
        {
            if (IsInBatchUpdate || _changedItems.Count <= 0)
                return;
            var formatMappingChanged = FormatMappingChanged;
            formatMappingChanged?.Invoke(this, new FormatItemsEventArgs(_changedItems.AsReadOnly()));
            _changedItems.Clear();
        }
    }
}