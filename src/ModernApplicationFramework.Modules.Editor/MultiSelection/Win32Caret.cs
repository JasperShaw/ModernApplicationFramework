using System;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Modules.Editor.NativeMethods;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class Win32Caret
    {
        private readonly IMultiSelectionBroker _broker;
        private readonly ITextView _textView;
        private IntPtr _windowHandle;
        private VirtualSnapshotPoint? _lastPosition;
        private bool _isCreated;

        public Win32Caret(ITextView textView, IMultiSelectionBroker broker)
        {
            _broker = broker;
            _textView = textView;
            _windowHandle = IntPtr.Zero;
            _isCreated = false;
            PresentationSource.AddSourceChangedHandler(_textView.VisualElement, OnPresentationSourceChanged);
            PresentationSource presentationSource = GetPresentationSource();
            if (presentationSource != null)
                GetWindowHandle(presentationSource);
            Create();
        }

        private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            Destroy();
            GetWindowHandle(e.NewSource);
            Create();
        }

        public void Update()
        {
            if (!(_windowHandle != IntPtr.Zero))
                return;
            SetPosition(_windowHandle);
        }

        public void Destroy()
        {
            if (!_isCreated || !(_windowHandle != IntPtr.Zero))
                return;
            User32.NotifyWinEvent(32769, _windowHandle, -8, 0);
            _isCreated = false;
        }

        public void Dispose()
        {
            PresentationSource.RemoveSourceChangedHandler(_textView.VisualElement, OnPresentationSourceChanged);
        }

        public void Show()
        {
            if (IsVisible)
                return;
            if (_windowHandle != IntPtr.Zero)
            {
                User32.NotifyWinEvent(32770, _windowHandle, -8, 0);
                IsVisible = true;
            }
            if (!IsVisible)
                return;
            SetPosition(_windowHandle);
        }

        public void Create()
        {
            if (!(_windowHandle != IntPtr.Zero))
                return;
            User32.NotifyWinEvent(32768, _windowHandle, -8, 0);
            _isCreated = true;
        }

        public void Hide()
        {
            if (!IsVisible)
                return;
            if (_windowHandle != IntPtr.Zero)
                User32.NotifyWinEvent(32771, _windowHandle, -8, 0);
            IsVisible = false;
        }

        internal bool IsShownOnScreen
        {
            get
            {
                AbstractSelectionPresentationProperties uiProperties = UiProperties;
                if (uiProperties == null)
                    return false;
                return uiProperties.IsWithinViewport;
            }
        }

        private AbstractSelectionPresentationProperties UiProperties
        {
            get
            {
                if (_broker.TryGetSelectionPresentationProperties(_broker.PrimarySelection, out var properties))
                    return properties;
                return null;
            }
        }

        private void SetPosition(IntPtr hWnd)
        {
            VirtualSnapshotPoint insertionPoint = _broker.PrimarySelection.InsertionPoint;
            VirtualSnapshotPoint? lastPosition = _lastPosition;
            VirtualSnapshotPoint virtualSnapshotPoint = insertionPoint;
            if ((lastPosition.HasValue ? (lastPosition.HasValue ? (lastPosition.GetValueOrDefault() != virtualSnapshotPoint ? 1 : 0) : 0) : 1) == 0)
                return;
            _lastPosition = new VirtualSnapshotPoint?(insertionPoint);
            if (UiProperties != null)
            {
                TextBounds caretBounds = UiProperties.CaretBounds;
                Rect rect = _textView.VisualElement.RenderTransform.TransformBounds(new Rect(caretBounds.Left, caretBounds.TextTop, caretBounds.Width, caretBounds.TextHeight));
                TopLeft = _textView.VisualElement.PointToScreen(rect.TopLeft);
                BottomRight = _textView.VisualElement.PointToScreen(rect.BottomRight);
            }
            User32.NotifyWinEvent(32779, hWnd, -8, 0);
        }

        private void GetWindowHandle(PresentationSource pSource)
        {
            _windowHandle = pSource is IWin32Window win32Window ? win32Window.Handle : IntPtr.Zero;
        }

        private PresentationSource GetPresentationSource()
        {
            new UIPermission(UIPermissionWindow.AllWindows).Assert();
            try
            {
                return PresentationSource.FromVisual(_textView.VisualElement);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        public bool IsVisible { get; private set; }

        public Point TopLeft { get; private set; }

        public Point BottomRight { get; private set; }
    }
}