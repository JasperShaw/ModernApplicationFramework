using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyboardSettingsViewModel : AbstractSettingsPage, IStretchSettingsPanelPanel
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IDialogProvider _dialogProvider;
        private readonly IKeyGestureService _gestureService;
        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;


        public ICommand ShowBindings => new Command(ExecuteMethod);

        private IEnumerable<CommandDefinition> _items;

        public IEnumerable<CommandDefinition> AllCommands
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IDialogProvider dialogProvider, IKeyGestureService gestureService)
        {
            _settingsManager = settingsManager;
            _dialogProvider = dialogProvider;
            _gestureService = gestureService;

            AllCommands = gestureService.GetAllCommandCommandDefinitions();
        }

        protected override bool SetData()
        {
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
        }


        private void ExecuteMethod()
        {
            var b = _gestureService.GetAllBindings();

            var message = b.Aggregate(string.Empty, (current, value) => current + (value + "\r\n"));

            _dialogProvider.ShowMessage(message);


            var ic = IoC.Get<ICommandService>().GetCommandDefinition(typeof(MultiUndoCommandDefinition)) as CommandDefinition;   
            if (ic == null)
                return;
            
        }
    }
}
