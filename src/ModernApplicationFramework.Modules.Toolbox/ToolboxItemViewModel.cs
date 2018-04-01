using System;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxItemViewModel
    {
        private readonly ToolboxItem _model;

        public ToolboxItem Model => _model;

        public string Name => _model.Name;

        public virtual string Category => _model.Category;

        public virtual Uri IconSource => _model.IconSource;

        public ToolboxItemViewModel(ToolboxItem model)
        {
            _model = model;
        }
    }
}
