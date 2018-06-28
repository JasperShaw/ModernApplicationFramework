using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.WindowManagement.LayoutManagement;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    /// <summary>
    /// Implementation to list all tool bars as a menu item. By clicking on the menu item the tool bars visibility will be toggled
    /// </summary>
    /// <seealso cref="ICommandListHandler{TCommandDefinition}" />
    [Export(typeof(ICommandHandler))]
    public class ShowLayoutsCommandHandler : ICommandListHandler<ListWindowLayoutsCommandDefinition>
    {
        private readonly IWindowLayoutStore _layoutStore;
        private readonly ApplyWindowLayout1 _command1;
        private readonly ApplyWindowLayout2 _command2;
        private readonly ApplyWindowLayout3 _command3;
        private readonly ApplyWindowLayout4 _command4;
        private readonly ApplyWindowLayout5 _command5;
        private readonly ApplyWindowLayout6 _command6;
        private readonly ApplyWindowLayout7 _command7;
        private readonly ApplyWindowLayout8 _command8;
        private readonly ApplyWindowLayout9 _command9;
        private readonly ApplyWindowLayout10 _command10;

        [ImportingConstructor]
        internal ShowLayoutsCommandHandler(IWindowLayoutStore layoutStore,
            ApplyWindowLayout1 command1,
            ApplyWindowLayout2 command2,
            ApplyWindowLayout3 command3,
            ApplyWindowLayout4 command4,
            ApplyWindowLayout5 command5,
            ApplyWindowLayout6 command6,
            ApplyWindowLayout7 command7,
            ApplyWindowLayout8 command8,
            ApplyWindowLayout9 command9,
            ApplyWindowLayout10 command10)
        {
            _layoutStore = layoutStore;
            _command1 = command1;
            _command2 = command2;
            _command3 = command3;
            _command4 = command4;
            _command5 = command5;
            _command6 = command6;
            _command7 = command7;
            _command8 = command8;
            _command9 = command9;
            _command10 = command10;
        }

        public void Populate(Command command, List<CommandDefinitionBase> commands)
        {
            if (LayoutManagementService.Instance == null)
                return;
            var layoutPairs = LayoutManagementUtilities.EnumerateLayoutKeyInfo(_layoutStore);
            var layouts = new ObservableCollection<LayoutItemViewModel>(layoutPairs
                .Select(kvp => new LayoutItemViewModel(kvp.Key, kvp.Value)).OrderBy(lvm => lvm.Position)
                .ThenBy(lvm => lvm.Name));
            if (layouts.Count == 0)
            {
                commands.Add(new ShowLayoutCommandDefinition(WindowManagement_Resources.NoSavedLayouts));
                return;
            }

            commands.AddRange(layouts.Select((t, i) => CreateCommand(i + 1, t.Name)));
        }


        private ApplyWindowLayoutBase CreateCommand(int index, string name)
        {
            switch (index)
            {
                case 1:
                    _command1.SetText(name);
                    return _command1;
                case 2:
                    _command2.SetText(name);
                    return _command2;
                case 3:
                    _command3.SetText(name);
                    return _command3;
                case 4:
                    _command4.SetText(name);
                    return _command4;
                case 5:
                    _command5.SetText(name);
                    return _command5;
                case 6:
                    _command6.SetText(name);
                    return _command6;
                case 7:
                    _command7.SetText(name);
                    return _command7;
                case 8:
                    _command8.SetText(name);
                    return _command8;
                case 9:
                    _command9.SetText(name);
                    return _command9;
                case 10:
                    _command10.SetText(name);
                    return _command10;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class ShowLayoutCommandDefinition : CommandDefinition
        {
            public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
            public override GestureScope DefaultGestureScope => null;
            public override string Name => string.Empty;
            public override string NameUnlocalized => string.Empty;
            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override CommandCategory Category => null;
            public override Guid Id => new Guid("{16ECEBFA-61B5-4C4E-ABAF-E64D4018B230}");

            public ShowLayoutCommandDefinition(string name)
            {
                Text = name;
            }
        }
    }
}