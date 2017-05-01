using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class CommandSplitButtonDefinition : CommandDefinition
    {
        public override CommandControlTypes ControlType => CommandControlTypes.SplitDropDown;

        public abstract IObservableCollection<IHasTextProperty> Items { get; set; }

        public abstract void Execute(int count);
    }
}
