using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace ModernApplicationFramework.Controls.AutomationPeer
{
    internal static class AutomationPeerHelper
    {
        internal static TPeerType CreatePeerFromElement<TPeerType>(UIElement element) where TPeerType : System.Windows.Automation.Peers.AutomationPeer
        {
            return UIElementAutomationPeer.CreatePeerForElement(element) as TPeerType;
        }

        internal static bool SelectionListenersExist()
        {
            if (!System.Windows.Automation.Peers.AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) && !System.Windows.Automation.Peers.AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection) && !System.Windows.Automation.Peers.AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
                return System.Windows.Automation.Peers.AutomationPeer.ListenerExists(AutomationEvents.SelectionPatternOnInvalidated);
            return true;
        }

        public static void RaiseSelectionEvents(System.Windows.Automation.Peers.AutomationPeer peer, SelectionChangedEventArgs e, object singleSelectedItem, bool childListenersMightExists, Func<object, System.Windows.Automation.Peers.AutomationPeer> getPeer)
        {
            if (!childListenersMightExists)
                peer.RaiseAutomationEvent(AutomationEvents.SelectionPatternOnInvalidated);
            else if (singleSelectedItem != null)
            {
                var automationPeer = getPeer(singleSelectedItem);
                if (automationPeer == null)
                    return;
                automationPeer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
            }
            else
            {
                var count1 = e.AddedItems.Count;
                var count2 = e.RemovedItems.Count;
                if (count1 + count2 > 20)
                    peer.RaiseAutomationEvent(AutomationEvents.SelectionPatternOnInvalidated);
                else
                {
                    for (int index = 0; index < count1; ++index)
                    {
                        var automationPeer = getPeer(e.AddedItems[index]);
                        automationPeer?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                    }
                    for (int index = 0; index < count2; ++index)
                    {
                        var automationPeer = getPeer(e.RemovedItems[index]);
                        automationPeer?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                    }
                }
            }
        }
    }
}
