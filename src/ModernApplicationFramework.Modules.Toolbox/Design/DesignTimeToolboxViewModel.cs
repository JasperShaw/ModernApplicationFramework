using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Modules.Toolbox.Design
{
    internal class DesignTimeToolboxViewModel : ToolboxViewModel
    {
        public DesignTimeToolboxViewModel()
            : base(null, null)
        {
            Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Foo", Category = "General" }));
            Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Bar", Category = "General" }));
            Items.Add(new ToolboxItemViewModel(new ToolboxItem { Name = "Baz", Category = "Misc" }));
        }
    }
}
