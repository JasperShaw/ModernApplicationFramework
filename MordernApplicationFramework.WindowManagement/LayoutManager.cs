using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
using MordernApplicationFramework.WindowManagement.LayoutManagement;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement
{
    [Export(typeof(ILayoutManager))]
    public class LayoutManager : ILayoutManager, ILayoutManagerInternal
    {
        private readonly ILayoutItemStatePersister _layoutItemStatePersister;
        private readonly IStatusBarDataModelService _statusBar;
        private readonly IWindowLayoutSettings _layoutSettings;
        private readonly IWindowLayoutStore _layoutStore;
        private readonly ILayoutManagementUserInput _layoutManagementUserInput;

        public int LayoutCount => _layoutStore.GetLayoutCount();

        [ImportingConstructor]
        internal LayoutManager(ILayoutItemStatePersister layoutItemStatePersister, IStatusBarDataModelService statusBar, 
            IWindowLayoutSettings layoutSettings)
        {
            Validate.IsNotNull(layoutSettings, nameof(layoutSettings));
            _layoutItemStatePersister = layoutItemStatePersister;
            _statusBar = statusBar;
            _layoutSettings = layoutSettings;
            _layoutManagementUserInput = new DialogUserInput(layoutSettings);

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
            var data = GetCurrentLayoutData();
            SaveWindowLayoutInternal(layoutName, data, hadNameConflict);
        }

        public void SaveWindowLayoutInternal(string layoutName, string layoutPayload, bool hadNameConflict)
        {
            var str = string.Empty;
            try
            {
                if (!hadNameConflict && 10 <= LayoutCount)
                {

                }
            }
            catch (Exception ex)
            {
                SetStatusBarMessage(LayoutManagerResources.SaveLayoutErrorStatusFormat, layoutName);
            }
        }

        private void SetStatusBarMessage(string format, string layoutName)
        {
            _statusBar?.SetText(string.Format(CultureInfo.CurrentUICulture, format, new object[]
            {
                layoutName
            }));
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

        private string GetCurrentLayoutData()
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Seek(0L, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        private class DialogUserInput : ILayoutManagementUserInput
        {
            private readonly IWindowLayoutSettings _settings;

            public DialogUserInput(IWindowLayoutSettings settings = null)
            {
                _settings = settings;
            }

            public bool TryGetSavedLayoutName(string defaultName, Predicate<string> nameValidator, out string layoutName)
            {
                Validate.IsNotNull(defaultName, nameof(defaultName));
                Validate.IsNotNull(nameValidator, nameof(nameValidator));
                return TextInputDialog.Show(WindowManagement_Resources.SaveLayoutCommandDefinitionMessageBox_Title,
                    WindowManagement_Resources.SaveLayoutCommandDefinitionMessageBox_Label, 100, defaultName,
                    nameValidator, out layoutName);
            }

            public bool TryGetApplyLayoutConfirmation(string name, out bool disableConfirmation)
            {
                if (MessageDialog.Show(WindowManagement_Resources.ApplyLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.ApplyLayoutConfirmation, new object[1]
                {
                    name
                }), MessageDialogCommandSet.OkCancel, WindowManagement_Resources.DisableApplyLayoutWarning, out disableConfirmation) != MessageDialogCommand.Cancel)
                    return true;
                disableConfirmation = false;
                return false;
            }

            public bool TryGetOverwriteLayoutConfirmation(string name)
            {
                return MessageDialog.Show(WindowManagement_Resources.SaveLayoutTitle, string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.LayoutOverwriteMessage, new object[]
                {
                    name
                }), MessageDialogCommandSet.YesNo) == MessageDialogCommand.Yes;
            }

            public IEnumerable<KeyValuePair<string, WindowLayoutInfo>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> layoutKeyInfoCollection)
            {
                return ManageLayoutsDialog.Show(layoutKeyInfoCollection, _settings);
            }

            public void ShowApplyLayoutError(string name)
            {
                DisplayError(string.Format(CultureInfo.CurrentUICulture, WindowManagement_Resources.ApplyLayoutFailedMessage, new object[]
                {
                    name
                }));
            }

            public void ShowSaveLayoutError(string message)
            {
                DisplayError(message);
            }

            private void DisplayError(string message)
            {
                Guid empty = Guid.Empty;
                int pnResult;
                //GlobalServices.UIShell.ShowMessageBox(0U, ref empty, (string)null, message, (string)null, 0U, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_CRITICAL, 0, out pnResult);
            }
        }
    }
}
