using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
using MordernApplicationFramework.WindowManagement.LayoutState;
using MordernApplicationFramework.WindowManagement.Properties;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    [Export(typeof(ILayoutManager))]
    public class LayoutManager : ILayoutManager, ILayoutManagerInternal
    {
        private readonly IStatusBarDataModelService _statusBar;
        private readonly IWindowLayoutSettings _layoutSettings;
        private readonly IWindowLayoutStore _layoutStore;
        private readonly ILayoutManagementUserInput _layoutManagementUserInput;
        private readonly ILayoutItemStatePersister _layoutStatePersister;

        public int LayoutCount => _layoutStore.GetLayoutCount();

        [ImportingConstructor]
        internal LayoutManager(IStatusBarDataModelService statusBar,
            IWindowLayoutSettings layoutSettings, IWindowLayoutStore layoutStore,
            ILayoutItemStatePersister statePersister)
        {
            Validate.IsNotNull(layoutSettings, nameof(layoutSettings));
            _statusBar = statusBar;
            _layoutSettings = layoutSettings;
            _layoutManagementUserInput = new DialogUserInput(layoutSettings);
            _layoutStore = layoutStore;
            _layoutStatePersister = statePersister;
        }

        public string GetLayoutNameAt(int index)
        {
            Validate.IsWithinRange(index, 0, LayoutCount - 1, nameof(index));
            return _layoutStore.GetLayoutAt(index).Value.Name;
        }

        public string GetLayoutDataAt(int index)
        {
            Validate.IsWithinRange(index, 0, LayoutCount - 1, nameof(index));
            return _layoutStore.GetLayoutDataAt(index);
        }

        public void ApplyWindowLayout(int index)
        {
            Validate.IsWithinRange(index, 0, LayoutCount - 1, nameof(index));
            var layoutAt = _layoutStore.GetLayoutAt(index);
            var name = layoutAt.Value.Name;
            if (!_layoutSettings.SkipApplyLayoutConfirmation)
            {
                if (!_layoutManagementUserInput.TryGetApplyLayoutConfirmation(name, out var disableConfirmation))
                    return;
                if (disableConfirmation)
                    _layoutSettings.SkipApplyLayoutConfirmation = true;
            }
            if (TryApplyWindowLayout(name, index))
                return;
            _layoutManagementUserInput.ShowApplyLayoutError(name);
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
            try
            {
                if (!hadNameConflict && 10 <= LayoutCount)
                {
                    _layoutManagementUserInput.ShowSaveLayoutError(string.Format(CultureInfo.CurrentUICulture,
                        WindowManagement_Resources.SaveLayoutErrorMaxLayoutsReached, new object[]
                        {
                            10
                        }));
                }
                else
                {
                    SetStatusBarMessage(WindowManagement_Resources.SaveLayoutStartedStatusFormat, layoutName);
                    _layoutStore.SaveLayout(layoutName, layoutPayload);
                    SetStatusBarMessage(WindowManagement_Resources.SaveLayoutCompletedStatusFormat, layoutName);
                }
            }
            catch (Exception)
            {
                SetStatusBarMessage(WindowManagement_Resources.SaveLayoutErrorStatusFormat, layoutName);
            }
        }

        private void SetStatusBarMessage(string format, string layoutName)
        {
            _statusBar?.SetText(string.Format(CultureInfo.CurrentUICulture, format, new object[]
            {
                layoutName
            }));
        }

        public void ManageWindowLayoutsInternal(Func<IEnumerable<KeyValuePair<string, WindowLayout>>, IEnumerable<KeyValuePair<string, WindowLayout>>> layoutTransformation)
        {
            var keyValuePairs = LayoutManagementUtilities.EnumerateLayoutKeyInfo(_layoutStore);
            _layoutStore.UpdateLayouts(layoutTransformation(keyValuePairs));
        }

        public void ApplyWindowLayoutInternal(int index)
        {
            Validate.IsWithinRange(index, 0, LayoutCount - 1, nameof(index));
            var layoutAt = _layoutStore.GetLayoutAt(index);
            var name = layoutAt.Value.Name;
            TryApplyWindowLayout(name, index);
        }

        private bool TryApplyWindowLayout(string name, int index)
        {
            var layoutDataAt = GetLayoutDataAt(index);
            if (string.IsNullOrEmpty(layoutDataAt))
                return false;

            SetStatusBarMessage(WindowManagement_Resources.ApplyLayoutStartedStatusFormat, name);
            try
            {
                using (var stream = LayoutManagementUtilities.ConvertLayoutPayloadToStream(layoutDataAt))
                {
                    var dh = IoC.Get<IDockingHostViewModel>();
                    if (dh == null)
                        throw new ArgumentNullException();
                    _layoutStatePersister.LoadFromStream(stream, ProcessStateOption.ToolsOnly);
                }
                SetStatusBarMessage(WindowManagement_Resources.ApplyLayoutCompletedStatusFormat, name);
            }
            catch (Exception)
            {
                SetStatusBarMessage(WindowManagement_Resources.ApplyLayoutErrorStatusFormat, name);
                return false;
            }
            return true;
        }

        private int GetDefaultLayoutNameIndex(string defaultNameFormat)
        {
            var num = 0;
            string name;
            do
            {
                ++num;
                name = string.Format(CultureInfo.CurrentUICulture, defaultNameFormat, new object[]
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
                _layoutStatePersister.SaveToStream(memoryStream, ProcessStateOption.ToolsOnly);
                memoryStream.Seek(0L, SeekOrigin.Begin);
                var byteArray = memoryStream.ToArray();
                return LayoutManagementUtilities.ConvertLayoutStreamToString(byteArray);
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

            public IEnumerable<KeyValuePair<string, WindowLayout>> ShowManageLayoutsView(IEnumerable<KeyValuePair<string, WindowLayout>> layoutKeyInfoCollection)
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

            private static void DisplayError(string message)
            {
                MessageBox.Show(message, null, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
    }
}
