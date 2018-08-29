using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Input.KeyBindingScheme
{
    [Export(typeof(IKeyBindingManager))]
    internal class KeyBindingManager : IKeyBindingManager
    {
        private readonly ICommandBarItemService _commandBarItemService;
        private readonly KeyBindingsSettings _keyBindingsSettings;
        private readonly IApplicationEnvironment _environment;

        public IKeyBindingSchemeManager SchemeManager { get; }
        public IKeyGestureService KeyGestureService { get; }

        [ImportingConstructor]
        public KeyBindingManager(IKeyGestureService gestureService, IKeyBindingSchemeManager schemeManager,
            KeyBindingsSettings keyBindingsSettings, IApplicationEnvironment environment,
            ICommandBarItemService commandBarItemService)
        {
            KeyGestureService = gestureService;
            SchemeManager = schemeManager;
            _keyBindingsSettings = keyBindingsSettings;
            _environment = environment;
            _commandBarItemService = commandBarItemService;

            gestureService.Initialize();
            schemeManager.LoadSchemeDefinitions();

            if (environment.UseApplicationSettings)
                ApplyKeyBindingsFromSettings();
            else
                LoadDefaultScheme();
        }

        public void LoadDefaultScheme()
        {
            ResetToKeyScheme(SchemeManager.SchemeDefinitions.FirstOrDefault());
        }

        public void SetKeyScheme(SchemeDefinition selectedScheme)
        {
            if (selectedScheme == null)
                return;
            var scheme = selectedScheme.Load();
            var allScopes = IoC.GetAll<GestureScope>().ToList();

            ResetToKeyScheme(selectedScheme);

            if (!_environment.UseApplicationSettings)
                return;

            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut != null)
                foreach (var shortcutsData in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut)
                {
                    var cmapping = GetCommandMapping(shortcutsData, allScopes);
                    if (cmapping == null)
                        continue;
                    if (!scheme.KeyGestureScopeMappings.Contains(
                        new CommandGestureScopeMapping(cmapping.CommandDefinition, cmapping.GestureScopeMapping)))
                        cmapping.CommandDefinition.Gestures.Insert(0, cmapping.GestureScopeMapping);
                }
            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut != null)
                foreach (var shortcutsData in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut)
                {
                    var cmapping = GetCommandMapping(shortcutsData, allScopes);
                    if (cmapping == null)
                        continue;
                    if (scheme.KeyGestureScopeMappings.Contains(
                        new CommandGestureScopeMapping(cmapping.CommandDefinition, cmapping.GestureScopeMapping)))
                        cmapping.CommandDefinition.Gestures.Remove(cmapping.GestureScopeMapping);
                }
        }

        public void ResetToKeyScheme(SchemeDefinition selectedScheme)
        {
            if (selectedScheme == null)
                return;
            var scheme = selectedScheme.Load();
            KeyGestureService.RemoveAllKeyGestures();
            foreach (var mapping in scheme.KeyGestureScopeMappings)
                mapping.CommandDefinition.Gestures.Add(mapping.GestureScopeMapping);
            SchemeManager.SetScheme(selectedScheme);
            if (_environment.UseApplicationSettings)
                _keyBindingsSettings.KeyboardShortcuts.ShortcutsScheme = selectedScheme.Name;
        }

        public void ApplyKeyBindingsFromSettings()
        {
            if (!_environment.UseApplicationSettings)
                throw new NotSupportedException(KeyBindingResources.Exception_SettingsNotEnabled);
            if (_keyBindingsSettings.KeyboardShortcuts == null)
                throw new ArgumentNullException(KeyBindingResources.Exception_NoSettingsFound);
            var scheme =
                SchemeManager.SchemeDefinitions.FirstOrDefault(x =>
                    x.Name == _keyBindingsSettings.KeyboardShortcuts.ShortcutsScheme);
            ResetToKeyScheme(scheme);

            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts == null)
                return;
            var allScopes = IoC.GetAll<GestureScope>().ToList();

            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut != null)
                foreach (var shortcut in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.Shortcut)
                {
                    var cmapping = GetCommandMapping(shortcut, allScopes);
                    cmapping?.CommandDefinition.Gestures.Insert(0, cmapping.GestureScopeMapping);
                }
            if (_keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut != null)
                foreach (var shortcut in _keyBindingsSettings.KeyboardShortcuts.UserShortcuts.RemoveShortcut)
                {
                    var cmapping = GetCommandMapping(shortcut, allScopes);
                    cmapping?.CommandDefinition.Gestures.Remove(cmapping.GestureScopeMapping);
                }
        }


        public void SaveCurrent()
        {
            if (!_environment.UseApplicationSettings)
                throw new NotSupportedException(KeyBindingResources.Exception_SettingsNotEnabled);
            _keyBindingsSettings.StoreSettings();
        }

        public void BuildCurrent()
        {
            if (!_environment.UseApplicationSettings)
                throw new NotSupportedException(KeyBindingResources.Exception_SettingsNotEnabled);
            _keyBindingsSettings.KeyboardShortcuts.ScopeDefinitions = KeyGestureService.GetAllCommandGestureScopes()
                .Select(scope => new KeyboardShortcutsScope(scope)).ToArray();

            _keyBindingsSettings.KeyboardShortcuts.ShortcutsScheme = SchemeManager.CurrentScheme?.Name;

            var additionalBindings = FindAditionalKeyBindings();
            var omittedBindings = FindOmittedKeyBindings();


            _keyBindingsSettings.KeyboardShortcuts.UserShortcuts =
                new KeyboardShortcutsUserShortcuts
                {
                    Shortcut = additionalBindings.ToArray(),
                    RemoveShortcut = omittedBindings.ToArray()
                };
        }

        private GestureScopeMapping GetMapping(KeyboardShortcutsUserShortcutsData shortcut,
            IEnumerable<GestureScope> allScopes)
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

            MultiKeyGesture gesture;
            try
            {
                gesture = (MultiKeyGesture) new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.CurrentCulture,
                    shortcut.Value);
            }
            catch (NotSupportedException e)
            {
                gesture = (MultiKeyGesture)new MultiKeyGestureConverter().ConvertFrom(null, CultureInfo.InvariantCulture,
                    shortcut.Value);
            }
            return new GestureScopeMapping(scope, gesture);
        }

        private CommandGestureScopeMapping GetCommandMapping(KeyboardShortcutsUserShortcutsData shortcut,
            IEnumerable<GestureScope> allScopes)
        {
            var cdb = _commandBarItemService.GetItemDefinition("CU", shortcut.Command);
            if (cdb == null || !(cdb is CommandDefinition cb))
                return null;
            var mapping = GetMapping(shortcut, allScopes);
            return mapping == null ? null : new CommandGestureScopeMapping(cb, mapping);
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsData> FindOmittedKeyBindings()
        {
            if (SchemeManager.CurrentScheme == null)
                return new List<KeyboardShortcutsUserShortcutsData>();
            var bindings = KeyGestureService.GetAllBindings().ToList();
            var cs = SchemeManager.CurrentScheme.Load();

            return (from mapping in cs.KeyGestureScopeMappings
                where !bindings.Any(x =>
                    x.CommandDefinition == mapping.CommandDefinition &&
                    x.GestureScopeMapping.Equals(mapping.GestureScopeMapping))
                select new KeyboardShortcutsUserShortcutsData
                {
                    Command = mapping.CommandDefinition.TrimmedCategoryCommandNameUnlocalized,
                    Scope = mapping.GestureScopeMapping.Scope.Text,
                    Value = mapping.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                }).ToList();
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsData> FindAditionalKeyBindings()
        {
            if (SchemeManager.CurrentScheme != null)
            {
                var bindings = KeyGestureService.GetAllBindings().ToList();
                var cs = SchemeManager.CurrentScheme.Load();
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
            else
            {
                var bindings = KeyGestureService.GetAllBindings().ToList();
                return bindings.Select(binding => new KeyboardShortcutsUserShortcutsData
                    {
                        Command = binding.CommandDefinition.TrimmedCategoryCommandNameUnlocalized,
                        Scope = binding.GestureScopeMapping.Scope.Text,
                        Value = binding.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                    })
                    .ToList();
            }
        }
    }
}