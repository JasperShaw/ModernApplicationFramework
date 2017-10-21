using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Extended.Interfaces;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement
{
    [Export(typeof(ILayoutManager))]
    public class LayoutManager : ILayoutManager, ILayoutManagerInternal
    {
        private readonly ILayoutItemStatePersister _layoutItemStatePersister;
        private readonly IWindowLayoutStore _layoutStore;

        public int LayoutCount => _layoutStore.GetLayoutCount();

        [ImportingConstructor]
        public LayoutManager(ILayoutItemStatePersister layoutItemStatePersister)
        {
            _layoutItemStatePersister = layoutItemStatePersister;

            _layoutStore = new WindowLayoutStore();
        }

        public string GetLayoutNameAt(int index)
        {
            throw new NotImplementedException();
        }

        public string GetLayoutDataAt(int index)
        {
            throw new NotImplementedException();
        }

        public void ApplyWindowLayout(int index)
        {
            throw new NotImplementedException();
        }

        public void SaveWindowLayout()
        {
            var layoutNameFormat = WindowManagement_Resources.SaveLayoutCommandDefinitionMessageBox_Default;
            var defaultLayoutNameIndex = GetDefaultLayoutNameIndex(layoutNameFormat);
            var defaultName = string.Format(CultureInfo.CurrentUICulture, layoutNameFormat, new object[]
            {
                defaultLayoutNameIndex
            });
            var hadNameConflict = false;
            if (!LayoutManagementDialogUserInput.TryGetSavedLayoutName(defaultName, name => ValidateLayoutName(name, out hadNameConflict), out string layoutName))
                return;
            SaveWindowLayoutInternal(LayoutManagementUtilities.NormalizeName(layoutName), hadNameConflict);
        }

        public void ManageWindowLayouts()
        {
            ManageWindowLayoutsInternal(LayoutManagementDialogUserInput.ShowManageLayoutsView);
        }

        public void SaveWindowLayoutInternal(string layoutName, bool hadNameConflict)
        {

        }

        public void SaveWindowLayoutInternal(string layoutName, string layoutPayload, bool hadNameConflict)
        {

        }

        public void ManageWindowLayoutsInternal(Func<IEnumerable<KeyValuePair<string, WindowLayoutInfo>>, IEnumerable<KeyValuePair<string, WindowLayoutInfo>>> layoutTransformation)
        {
            var keyValuePairs = LayoutManagementUtilities.EnumerateLayoutKeyInfo(_layoutStore);
            //TODO: Update LayoutStore
            layoutTransformation(keyValuePairs);
        }

        public void ApplyWindowLayoutInternal(int index)
        {
            throw new NotImplementedException();
        }

        private int GetDefaultLayoutNameIndex(string defaultNameFormat)
        {
            var num = 0;
            string name;
            do
            {
                ++num;
                name = string.Format(CultureInfo.CurrentUICulture, defaultNameFormat, new[]
                {
                    num
                });
            } while (num < 11 && !LayoutManagementUtilities.IsUniqueName(name, _layoutStore));
            return num;
        }

        private bool ValidateLayoutName(string name, out bool hadNameConflict)
        {
            name = LayoutManagementUtilities.NormalizeName(name);
            hadNameConflict = false;
            if (LayoutManagementUtilities.IsUniqueName(name, _layoutStore))
                return true;
            hadNameConflict = true;
            return LayoutManagementDialogUserInput.TryGetOverwriteLayoutConfirmation(name);
        }
    }
}
