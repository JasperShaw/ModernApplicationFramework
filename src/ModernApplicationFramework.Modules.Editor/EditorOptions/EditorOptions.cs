﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.EditorOptions
{
    internal class EditorOptions : IEditorOptions
    {
        private readonly FrugalList<WeakReference> _derivedEditorOptions = new FrugalList<WeakReference>();
        private readonly EditorOptionsFactoryService _factory;
        private EditorOptions _parent;

        public event EventHandler<EditorOptionChangedEventArgs> OptionChanged;

        public IEditorOptions GlobalOptions => _factory.GlobalOptions;

        public IEditorOptions Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                    return;
                if (_parent == null)
                    throw new InvalidOperationException("Cannot change the Parent of the global options.");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value == this)
                    throw new ArgumentException(
                        "The Parent of this instance of IEditorOptions cannot be set to itself.");
                var editorOptions = value as EditorOptions;
                if (editorOptions == null)
                    throw new ArgumentException(
                        "New parent must be an instance of IEditorOptions generated by the same factory as this instance.");
                var parent = _parent;
                _parent.RemovedDerivedOptions(this);
                _parent = editorOptions;
                _parent.AddDerivedOptions(this);
                CheckForCycle();
                foreach (var instantiatedOption in _factory.GetInstantiatedOptions(Scope))
                    if (!OptionsSetLocally.Contains(instantiatedOption.Name) && !Equals(
                            parent.GetOptionForChild(instantiatedOption),
                            _parent.GetOptionForChild(instantiatedOption)))
                        RaiseChangedEvent(instantiatedOption);
            }
        }

        public IEnumerable<EditorOptionDefinition> SupportedOptions => _factory.GetSupportedOptions(Scope);

        private HybridDictionary OptionsSetLocally { get; }

        private IPropertyOwner Scope { get; }

        internal EditorOptions(EditorOptions parent, IPropertyOwner scope, EditorOptionsFactoryService factory)
        {
            _parent = parent;
            _factory = factory;
            Scope = scope;
            OptionsSetLocally = new HybridDictionary();
            parent?.AddDerivedOptions(this);
        }

        public bool ClearOptionValue(string optionId)
        {
            if (Parent == null || !OptionsSetLocally.Contains(optionId))
                return false;
            var objA = OptionsSetLocally[optionId];
            OptionsSetLocally.Remove(optionId);
            var definitionOrThrow = _factory.GetOptionDefinitionOrThrow(optionId);
            var optionValue = GetOptionValue(definitionOrThrow);
            if (!Equals(objA, optionValue))
                RaiseChangedEvent(definitionOrThrow);
            return true;
        }

        public bool ClearOptionValue<T>(EditorOptionKey<T> key)
        {
            return ClearOptionValue(key.Name);
        }

        public T GetOptionValue<T>(string optionId)
        {
            var definitionOrThrow = _factory.GetOptionDefinitionOrThrow(optionId);
            if (!typeof(T).IsAssignableFrom(definitionOrThrow.ValueType))
                throw new InvalidOperationException("Invalid type requested for the given option.");
            return (T) GetOptionValue(definitionOrThrow);
        }

        public T GetOptionValue<T>(EditorOptionKey<T> key)
        {
            return GetOptionValue<T>(key.Name);
        }

        public object GetOptionValue(string optionId)
        {
            return GetOptionValue(_factory.GetOptionDefinitionOrThrow(optionId));
        }

        public bool IsOptionDefined(string optionId, bool localScopeOnly)
        {
            if (localScopeOnly && _parent != null)
                return OptionsSetLocally.Contains(optionId);
            var optionDefinition = _factory.GetOptionDefinition(optionId);
            return optionDefinition != null && (Scope == null || optionDefinition.IsApplicableToScope(Scope));
        }

        public bool IsOptionDefined<T>(EditorOptionKey<T> key, bool localScopeOnly)
        {
            if (localScopeOnly && _parent != null)
                return OptionsSetLocally.Contains(key.Name);
            var optionDefinition = _factory.GetOptionDefinition(key.Name);
            return optionDefinition != null && (Scope == null || optionDefinition.IsApplicableToScope(Scope)) &&
                   optionDefinition.ValueType.IsEquivalentTo(typeof(T));
        }

        public void SetOptionValue(string optionId, object value)
        {
            var definitionOrThrow = _factory.GetOptionDefinitionOrThrow(optionId);
            if (!definitionOrThrow.ValueType.IsInstanceOfType(value))
                throw new ArgumentException("Specified option value is of an invalid type", nameof(value));
            if (!definitionOrThrow.IsValid(ref value))
                throw new ArgumentException("The supplied value failed validation for the option.", nameof(value));
            var optionValue = GetOptionValue(definitionOrThrow);
            OptionsSetLocally[optionId] = value;
            var objB = value;
            if (Equals(optionValue, objB))
                return;
            RaiseChangedEvent(definitionOrThrow);
        }

        public void SetOptionValue<T>(EditorOptionKey<T> key, T value)
        {
            SetOptionValue(key.Name, value);
        }

        internal void AddDerivedOptions(EditorOptions derived)
        {
            _derivedEditorOptions.RemoveAll(weakref => !weakref.IsAlive);
            _derivedEditorOptions.Add(new WeakReference(derived));
        }

        internal object GetOptionForChild(EditorOptionDefinition definition)
        {
            if (OptionsSetLocally.Contains(definition.Name))
                return OptionsSetLocally[definition.Name];
            if (_parent == null)
                return definition.DefaultValue;
            return _parent.GetOptionForChild(definition);
        }

        internal void OnParentOptionChanged(EditorOptionDefinition definition)
        {
            if (OptionsSetLocally.Contains(definition.Name))
                return;
            RaiseChangedEvent(definition);
        }

        internal void RemovedDerivedOptions(EditorOptions derived)
        {
            foreach (var derivedEditorOption in _derivedEditorOptions)
                if (derivedEditorOption.Target == derived)
                {
                    _derivedEditorOptions.Remove(derivedEditorOption);
                    break;
                }
        }

        private void CheckForCycle()
        {
            var parent = _parent;
            var editorOptionsSet = new HashSet<EditorOptions>();
            for (; parent != null; parent = parent._parent)
            {
                if (editorOptionsSet.Contains(parent))
                    throw new ArgumentException("Cycles are not allowed in the Parent chain.");
                editorOptionsSet.Add(parent);
            }
        }

        private object GetOptionValue(EditorOptionDefinition definition)
        {
            if (!TryGetOption(definition, out var obj))
                throw new ArgumentException($"The specified option is not valid in this scope: {definition.Name}");
            return obj;
        }

        private void RaiseChangedEvent(EditorOptionDefinition definition)
        {
            if (Scope == null || definition.IsApplicableToScope(Scope))
            {
                var optionChanged = OptionChanged;
                optionChanged?.Invoke(this, new EditorOptionChangedEventArgs(definition.Name));
            }

            _derivedEditorOptions.RemoveAll(weakref => !weakref.IsAlive);
            foreach (var weakReference in new FrugalList<WeakReference>(_derivedEditorOptions))
                (weakReference.Target as EditorOptions)?.OnParentOptionChanged(definition);
        }

        private bool TryGetOption(EditorOptionDefinition definition, out object value)
        {
            value = null;
            if (Scope != null && !definition.IsApplicableToScope(Scope))
                return false;
            value = GetOptionForChild(definition);
            return true;
        }
    }
}