using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces;
using ISettingsPage = ModernApplicationFramework.Settings.Interfaces.ISettingsPage;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class SettingsPage : ViewModelBase, ISettingsPage
    {
        public abstract uint SortOrder { get; }
        public abstract string Name { get; }
        public abstract SettingsPageCategory Category { get; }

        public IDirtyObjectManager DirtyObjectManager { get; }

        IDirtyObjectManager ICanBeDirty.DirtyObjectManager => throw new NotImplementedException();

        protected SettingsPage()
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

        public virtual void Cancel()
        {
            if (DirtyObjectManager.IsDirty)
                RestoreData();
            DirtyObjectManager.Clear();
        }

        /// <summary>
        /// Sets the changed Values to the correct settings data model.
        /// </summary>
        /// <returns>Returns whether the progress was successful</returns>
        protected abstract bool SetData();

        /// <summary>
        /// Advanced Method to restore Data if dialog was canceled.
        /// </summary>
        protected virtual void RestoreData() { }

        public abstract bool CanApply();

        public abstract void Activate();
    }
}
