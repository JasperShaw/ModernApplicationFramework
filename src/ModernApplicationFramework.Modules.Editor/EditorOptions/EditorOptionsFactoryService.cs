﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.EditorOptions
{
    [Export(typeof(IEditorOptionsFactoryService))]
    internal sealed class EditorOptionsFactoryService : IEditorOptionsFactoryService
    {
        [Import] public GuardedOperations GuardedOperations;

        private readonly IDictionary<string, EditorOptionDefinition> _instantiatedOptionDefinitions =
            new Dictionary<string, EditorOptionDefinition>();

        private readonly IDictionary<string, Lazy<EditorOptionDefinition, INameMetadata>> _namedOptionImports =
            new Dictionary<string, Lazy<EditorOptionDefinition, INameMetadata>>();

        private EditorOptions _globalOptions;

        public IEditorOptions GlobalOptions
        {
            get
            {
                if (_globalOptions == null)
                {
                    _globalOptions = new EditorOptions(null, null, this);
                    Initialize();
                }

                return _globalOptions;
            }
        }

        [ImportMany(typeof(EditorOptionDefinition))]
        internal List<Lazy<EditorOptionDefinition, INameMetadata>> OptionImports { get; set; }

        public IEditorOptions CreateOptions()
        {
            return new EditorOptions(GlobalOptions as EditorOptions, null, this);
        }

        public IEditorOptions GetOptions(IPropertyOwner scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));
            return scope.Properties.GetOrCreateSingletonProperty(() =>
                (IEditorOptions) new EditorOptions(GlobalOptions as EditorOptions, scope, this));
        }

        internal IEnumerable<EditorOptionDefinition> GetInstantiatedOptions(IPropertyOwner scope)
        {
            List<EditorOptionDefinition> list;
            lock (_instantiatedOptionDefinitions)
            {
                list = _instantiatedOptionDefinitions.Values.ToList();
            }

            foreach (var optionDefinition in list)
                if (scope == null || optionDefinition.IsApplicableToScope(scope))
                    yield return optionDefinition;
        }

        internal EditorOptionDefinition GetOptionDefinition(string optionId)
        {
            lock (_instantiatedOptionDefinitions)
            {
                if (!_instantiatedOptionDefinitions.TryGetValue(optionId, out var optionDefinition) &&
                    _namedOptionImports.TryGetValue(optionId, out var provider))
                {
                    optionDefinition = GuardedOperations.InstantiateExtension(provider, provider);
                    _namedOptionImports.Remove(optionId);
                    _instantiatedOptionDefinitions.Add(optionId, optionDefinition);
                }

                return optionDefinition;
            }
        }

        internal EditorOptionDefinition GetOptionDefinitionOrThrow(string optionId)
        {
            var optionDefinition = GetOptionDefinition(optionId);
            if (optionDefinition != null)
                return optionDefinition;
            throw new ArgumentException($"No EditorOptionDefinition export found for the given option name: {optionId}",
                nameof(optionId));
        }

        internal IEnumerable<EditorOptionDefinition> GetSupportedOptions(IPropertyOwner scope)
        {
            lock (_instantiatedOptionDefinitions)
            {
                foreach (var namedOptionImport in _namedOptionImports)
                {
                    var optionDefinition =
                        GuardedOperations.InstantiateExtension(namedOptionImport.Value, namedOptionImport.Value);
                    SafeAdd(_instantiatedOptionDefinitions, namedOptionImport.Key, optionDefinition);
                }

                _namedOptionImports.Clear();
            }

            foreach (var optionDefinition in _instantiatedOptionDefinitions.Values)
                if (scope == null || optionDefinition.IsApplicableToScope(scope))
                    yield return optionDefinition;
        }

        private void Initialize()
        {
            foreach (var optionImport in OptionImports)
                if (optionImport.Metadata.Name != null)
                {
                    SafeAdd(_namedOptionImports, optionImport.Metadata.Name, optionImport);
                }
                else
                {
                    var optionDefinition = GuardedOperations.InstantiateExtension(optionImport, optionImport);
                    SafeAdd(_instantiatedOptionDefinitions, optionDefinition.Name, optionDefinition);
                }
        }

        private void SafeAdd<T>(IDictionary<string, T> dictionary, string name, T value)
        {
            try
            {
                dictionary.Add(name, value);
            }
            catch (ArgumentException)
            {
                GuardedOperations.HandleException(this,
                    new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "Duplicate EditorOptionDefinition named {0}", name)));
            }
        }
    }
}