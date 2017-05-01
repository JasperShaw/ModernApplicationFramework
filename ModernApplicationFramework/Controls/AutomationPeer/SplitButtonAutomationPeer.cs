using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;

namespace ModernApplicationFramework.Controls.AutomationPeer
{
    public class SplitButtonAutomationPeer : MenuItemAutomationPeer, IInvokeProvider, ISelectionProvider, IValueProvider
    {
        bool ISelectionProvider.CanSelectMultiple => true;

        bool ISelectionProvider.IsSelectionRequired => true;

        bool IValueProvider.IsReadOnly => true;

        string IValueProvider.Value
        {
            get
            {
                var owner = (SplitButton) Owner;
                if (!owner.IsEnabled)
                    return string.Empty;
                owner.UpdateChildCollection();
                if (owner.DataContext == null || owner.SelectedIndex < 0)
                    return string.Empty;
                return "Test";
            }
        }

        public SplitButtonAutomationPeer(SplitButton splitButton) : base(splitButton)
        {    
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.SplitButton;
        }

        protected override string GetClassNameCore()
        {
            return "SplitButton";
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
                return this;
            if (patternInterface == PatternInterface.Selection)
                return this;
            if (patternInterface == PatternInterface.Value)
                return this;
            return base.GetPattern(patternInterface);
        }

        void IInvokeProvider.Invoke()
        {
            ((SplitButton)Owner).Invoke();
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            var owner = (SplitButton) Owner;
            if (!owner.IsEnabled)
                return new IRawElementProviderSimple[0];
            owner.UpdateChildCollection();
            if (!owner.IsSubmenuOpen)
                return new IRawElementProviderSimple[0];
            var containerGenerator = (IItemContainerGenerator) owner.ItemContainerGenerator;
            var elementPProivderSimpleList = new List<IRawElementProviderSimple>();
            var num = 0;
            using (containerGenerator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
            {
                for (var next = (SplitButtonItem)containerGenerator.GenerateNext(); next != null; next = (SplitButtonItem)containerGenerator.GenerateNext())
                {
                    if (num++ < owner.SelectedIndex)
                    {
                        var peerForElement = CreatePeerForElement(next);
                        elementPProivderSimpleList.Add(ProviderFromPeer(peerForElement));
                    }
                    else
                        break;
                }
            }
            return elementPProivderSimpleList.ToArray();
        }

        void IValueProvider.SetValue(string value)
        {
            throw new NotSupportedException();
        }

        private static object GetNthItemFromItemsCollection(uint n, object ds)
        {
            object pVsUIDataSource;
            //Utilities.QueryTypedValue<IVsUICollection>(ds, "Items")).GetItem(n, out pVsUIDataSource);
            //return pVsUIDataSource;

            return ds;
        }
    }
}
