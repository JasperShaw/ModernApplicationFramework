﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Basics.CommandBar.Models;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(TestMenuControllerDefinition))]
    public sealed class TestMenuControllerDefinition : MenuControllerDefinition
    {
        private readonly Lazy<MenuControllerModel> _model;
        public override string Name => "MenuController";
        public override string NameUnlocalized => Text;
        public override string Text => "MenuController";
        public override string ToolTip => null;

        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{624FF898-788B-476D-B269-879729431260}");

        public TestMenuControllerDefinition()
        {
            var items = new List<MenuControllerModel.MenuControllerModelItem>
            {
                new MenuControllerModel.MenuControllerModelItem(typeof(UndoCommandDefinition)),
                new MenuControllerModel.MenuControllerModelItem(typeof(OpenSettingsCommandDefinition))
            };
            _model = new Lazy<MenuControllerModel>(() => new MenuControllerModel(items));
        }

        public override MenuControllerModel Model => _model.Value;
    }
}