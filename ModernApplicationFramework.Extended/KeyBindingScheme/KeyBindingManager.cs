using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.KeyBindingScheme
{
    [Export(typeof(KeyBindingManager))]
    public class KeyBindingManager
    {
        private readonly IKeyGestureService _gestureService;
        private readonly IKeyBindingSchemeManager _schemeManager;
        private readonly KeyBindingsSettings _keyBindingsSettings;
        private readonly IApplicationEnvironment _environment;
        private readonly ICommandService _commandService;

        [ImportingConstructor]
        public KeyBindingManager(IKeyGestureService gestureService, IKeyBindingSchemeManager schemeManager, 
            KeyBindingsSettings keyBindingsSettings, IApplicationEnvironment environment, ICommandService commandService)
        {
            _gestureService = gestureService;
            _schemeManager = schemeManager;
            _keyBindingsSettings = keyBindingsSettings;
            _environment = environment;
            _commandService = commandService;

            _gestureService.Initialize();
            _schemeManager.LoadSchemeDefinitions();

            if (_environment.UseApplicationSettings)
                ApplyKeyBindingsFromSettings();
            else
                LoadDefaultScheme();
        }

        public void LoadDefaultScheme()
        {
            ResetToKeyScheme(_schemeManager.SchemeDefinitions.FirstOrDefault());
        }

        public void SetKeyScheme(SchemeDefinition selectedScheme)
        {

        }

        public void ResetToKeyScheme(SchemeDefinition selectedScheme)
        {
            if (selectedScheme == null)
                return;
            var scheme = selectedScheme.Load();
            _gestureService.RemoveAllKeyGestures();
            foreach (var mapping in scheme.KeyGestureScopeMappings)
                mapping.CommandDefinition.Gestures.Add(mapping.GestureScopeMapping);
            _schemeManager.SetScheme(selectedScheme);
        }

        public void ApplyKeyBindingsFromSettings()
        {
            if (_keyBindingsSettings.KeyboardShortcuts == null)
                throw new ArgumentNullException("Keyboard settings data was null");
            var scheme = _schemeManager.SchemeDefinitions.First(x => x.Name == _keyBindingsSettings.KeyboardShortcuts.ShortcutsScheme);
            ResetToKeyScheme(scheme);

            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts == null)
                return;
            var allScopes = IoC.GetAll<GestureScope>().ToList();

            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut != null)
            {
                foreach (var shortcut in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut)
                {
                    var cdb = _commandService.GetCommandDefinitionBy("CU", shortcut.Command);
                    if (cdb == null || !(cdb is CommandDefinition cb))
                        continue;
                    var mapping = GetMapping(shortcut, allScopes);
                    if (mapping == null)
                        continue;
                    cb.Gestures.Insert(0, mapping);
                }
            }
            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut != null)
            {
                foreach (var shortcut in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut)
                {
                    var cdb = _commandService.GetCommandDefinitionBy("CU", shortcut.Command);
                    if (cdb == null || !(cdb is CommandDefinition cb))
                        continue;
                    var mapping = GetMapping(shortcut, allScopes);
                    if (mapping == null)
                        continue;
                    cb.Gestures.Remove(mapping);
                }
            }
        }

        private GestureScopeMapping GetMapping(KeyboardShortcutsUserShortcutsData shortcut, IEnumerable<GestureScope> allScopes)
        {
            var scopeId =
                _keyBindingsSettings.KeyboardShortcuts.ScopeDefinitions.FirstOrDefault(
                        x => x.Name == shortcut.Scope)
                    ?.ID;
            if (string.IsNullOrEmpty(scopeId))
                return null;
            var scope = allScopes.FirstOrDefault(x => x.Id.Equals(new Guid(scopeId)));
            if (scope == null)
                return null;
            return new GestureScopeMapping(scope,
                (MultiKeyGesture)new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.InvariantCulture,
                    shortcut.Value));
        }



        public void SaveCurrent()
        {
            _keyBindingsSettings.KeyboardShortcuts.ScopeDefinitions = _gestureService.GetAllCommandGestureScopes()
                .Select(scope => new KeyboardShortcutsScope(scope)).ToArray();
            _keyBindingsSettings.KeyboardShortcuts.ShortcutsScheme = _schemeManager.CurrentScheme.Name;

            var additionalBindings = FindAditionalKeyBindings();
            var omittedBindings = FindOmittedKeyBindings();


            _keyBindingsSettings.KeyboardShortcuts.UserShortcuts =
                new KeyboardShortcutsUserShortcuts
                {
                    Shortcut = additionalBindings.ToArray(),
                    RemoveShortcut = omittedBindings.ToArray()
                };
            _keyBindingsSettings.StoreSettings();
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsData> FindOmittedKeyBindings()
        {
            if (_schemeManager.CurrentScheme == null)
                throw new NullReferenceException(nameof(_schemeManager.CurrentScheme));
            var bindings = _gestureService.GetAllBindings().ToList();
            var cs = _schemeManager.CurrentScheme.Load();

            return (from mapping in cs.KeyGestureScopeMappings
                where !bindings.Any(x => x.CommandDefinition == mapping.CommandDefinition && x.GestureScopeMapping.Equals(mapping.GestureScopeMapping))
                select new KeyboardShortcutsUserShortcutsData
                {
                    Command = mapping.CommandDefinition.TrimmedCategoryCommandNameUnlocalized,
                    Scope = mapping.GestureScopeMapping.Scope.Text,
                    Value = mapping.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                }).ToList();
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsData> FindAditionalKeyBindings()
        {
            if (_schemeManager.CurrentScheme == null)
                throw new NullReferenceException(nameof(_schemeManager.CurrentScheme));
            var bindings = _gestureService.GetAllBindings().ToList();
            var cs = _schemeManager.CurrentScheme.Load();
            return (from binding in bindings
                where !cs.KeyGestureScopeMappings.Any(x =>
                    x.CommandDefinition == binding.CommandDefinition &&
                    x.GestureScopeMapping.Equals(binding.GestureScopeMapping))
                select new KeyboardShortcutsUserShortcutsData
                {
                    Command = binding.CommandDefinition.TrimmedCategoryCommandNameUnlocalized,
                    Scope = binding.GestureScopeMapping.Scope.Text,
                    Value = binding.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                }).ToList();
        }
    }
}
