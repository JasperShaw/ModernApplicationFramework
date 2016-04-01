using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.MVVM.Demo.Modules;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo
{
    public class ToolbarDefinitionsPopulator : ToolbarDefinitionsPopulatorBase
    {
        public override IList<ToolbarDefinition> GetDefinitions()
        {
            var t1 = new ToolbarDefinition(new TestToolBar(), 0, true, Dock.Top);

            ToolbarDefinition.Add(t1);

            return ToolbarDefinition;
        }
    }
}
