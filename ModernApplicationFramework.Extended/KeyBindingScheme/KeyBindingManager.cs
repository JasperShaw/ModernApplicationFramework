using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Extended.Interfaces;
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

        [ImportingConstructor]
        public KeyBindingManager(IKeyGestureService gestureService, IKeyBindingSchemeManager schemeManager, 
            KeyBindingsSettings keyBindingsSettings, IApplicationEnvironment environment)
        {
            _gestureService = gestureService;
            _schemeManager = schemeManager;
            _keyBindingsSettings = keyBindingsSettings;
            _environment = environment;

            _gestureService.Initialize();
            _schemeManager.LoadSchemeDefinitions();

            if (_environment.UseApplicationSettings)
                ApplyKeyBindingsFromSettings();
            else
                LoadDefaultScheme();
        }

        public void LoadDefaultScheme()
        {
            _schemeManager.SetDefaultScheme();
        }

        public void ApplyKeyBindingsFromSettings()
        {
            _schemeManager.SetDefaultScheme();
        }

        public void ResetToKeyScheme()
        {
            
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

        public void SetKeyScheme(SchemeDefinition selectedScheme)
        {
            //TODO
            _schemeManager.ResetToScheme(selectedScheme);
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsRemoveShortcut> FindOmittedKeyBindings()
        {
            if (_schemeManager.CurrentScheme == null)
                throw new NullReferenceException(nameof(_schemeManager.CurrentScheme));
            var bindings = _gestureService.GetAllBindings().ToList();
            var cs = _schemeManager.CurrentScheme.Load();

            return (from mapping in cs.KeyGestureScopeMappings
                where !bindings.Any(x => x.CommandDefinition == mapping.CommandDefinition && x.GestureScopeMapping.Equals(mapping.GestureScopeMapping))
                select new KeyboardShortcutsUserShortcutsRemoveShortcut
                {
                    Command = mapping.CommandDefinition.TrimmedCategoryCommandName,
                    Scope = mapping.GestureScopeMapping.Scope.Text,
                    Value = mapping.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                }).ToList();
        }

        private IEnumerable<KeyboardShortcutsUserShortcutsShortcut> FindAditionalKeyBindings()
        {
            if (_schemeManager.CurrentScheme == null)
                throw new NullReferenceException(nameof(_schemeManager.CurrentScheme));
            var bindings = _gestureService.GetAllBindings().ToList();
            var cs = _schemeManager.CurrentScheme.Load();
            return (from binding in bindings
                where !cs.KeyGestureScopeMappings.Any(x =>
                    x.CommandDefinition == binding.CommandDefinition &&
                    x.GestureScopeMapping.Equals(binding.GestureScopeMapping))
                select new KeyboardShortcutsUserShortcutsShortcut
                {
                    //Command = binding.CommandDefinition.ToString("C", CultureInfo.InvariantCulture),
                    Scope = binding.GestureScopeMapping.Scope.Text,
                    Value = binding.GestureScopeMapping.KeyGesture.GetCultureString(CultureInfo.InvariantCulture)
                }).ToList();
        }
    }
}
