using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IToolbarService))]
    public class ToolbarService : IToolbarService
    {
        private readonly Dictionary<Type, ToolBar> _commandDefinitionsLookup;

#pragma warning disable 649
        [ImportMany] private ToolBar[] _commandDefinitions;
#pragma warning restore 649

        public ToolbarService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, ToolBar>();
        }

        public ToolBar GetToolbar(Type toolBartype)
        {
            ToolBar toolBar;
            if (!_commandDefinitionsLookup.TryGetValue(toolBartype, out toolBar))
                toolBar = _commandDefinitionsLookup[toolBartype] =
                    _commandDefinitions.First(x => x.GetType() == toolBartype);
            return toolBar;
        }
    }
}