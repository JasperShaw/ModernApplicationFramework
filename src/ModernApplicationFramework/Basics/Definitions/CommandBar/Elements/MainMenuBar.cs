using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Elements
{
    public sealed class MainMenuBar : CommandBarItem
    {
        public static MainMenuBar Instance { get; private set; }

        public MainMenuBar(string name)
        {
            if (Instance != null)
                throw new ArgumentException("Main Menu bar already exists");
            ItemDataSource = new MenuBarDataSource(new Guid("{E3C38E3A-272D-4FB5-BA8A-208FFF5142AE}"), name);
            Instance = this;
        }

        public override CommandBarDataSource ItemDataSource { get; }
    }
}
