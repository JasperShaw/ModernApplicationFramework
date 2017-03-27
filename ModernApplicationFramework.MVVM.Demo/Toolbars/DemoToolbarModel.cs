using System.Collections.ObjectModel;
using Caliburn.Micro;
using Button = System.Windows.Controls.Button;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public class DemoToolbarModel
    {
        static DemoToolbarModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(DemoToolbarModel).Namespace,
                typeof(DemoToolbarModel).Namespace);
        }

        public DemoToolbarModel()
        {
            //Items = new ObservableCollection<Button>();

            //var b = new CommandDefinitionButton<RedoCommandDefinition>();
            //var bc = new CommandDefinitionButton<UndoCommandDefinition>();

            //Items.Add(b);
            //Items.Add(bc);
        }

        public ObservableCollection<Button> Items { get; set; }


    }
}