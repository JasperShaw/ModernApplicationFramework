using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;
using ISettingsCategory = ModernApplicationFramework.Settings.Interfaces.ISettingsCategory;
using ISettingsPage = ModernApplicationFramework.Settings.Interfaces.ISettingsPage;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class AbstractSettingsPage : ViewModelBase, ISettingsPage
    {
        public abstract uint SortOrder { get; }
        public abstract string Name { get; }
        public abstract ISettingsCategory Category { get; }

        public IDirtyObjectManager DirtyObjectManager { get; }

        IDirtyObjectManager ICanBeDirty.DirtyObjectManager => throw new NotImplementedException();

        protected AbstractSettingsPage()
        {
            DirtyObjectManager = new DirtyObjectManager();
        }


        public bool Apply()
        {
            bool status = true;
            if (DirtyObjectManager.IsDirty)
                status = SetData();
            if (!status)
                return false;
            DirtyObjectManager.Clear();
            return true;
        }

        /// <summary>
        /// Sets the changed Values to the correct settings data model.
        /// </summary>
        /// <returns>Returns whether the progress was successful</returns>
        protected abstract bool SetData();

        public abstract bool CanApply();

        public abstract void Activate();
    }
}
