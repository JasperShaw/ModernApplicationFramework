using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    public class ToolBarOverflowPopup : Popup
    {
        protected override void OnOpened(EventArgs e)
        {
            SetAutomationPropertiesToLogicalChild();
            base.OnOpened(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (Child.IsKeyboardFocusWithin)
                Keyboard.Focus(null);
            base.OnClosed(e);
        }

        private void SetAutomationPropertiesToLogicalChild()
        {
            PresentationSource presentationSource = PresentationSource.FromVisual(Child);
            if (presentationSource == null)
                return;
            PropagateAutomationProperties(presentationSource.RootVisual);
        }

        protected virtual void PropagateAutomationProperties(Visual popupRoot)
        {
            if (popupRoot == null)
                throw new ArgumentNullException(nameof(popupRoot));
            popupRoot.SetValue(AutomationProperties.NameProperty, GetValue(AutomationProperties.NameProperty));
            popupRoot.SetValue(AutomationProperties.AutomationIdProperty, GetValue(AutomationProperties.AutomationIdProperty));
        }
    }
}
