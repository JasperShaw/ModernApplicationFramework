using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        public static readonly DependencyProperty PopupAnimationProperty;


        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu),
                new FrameworkPropertyMetadata(typeof(ContextMenu)));
            PopupAnimationProperty = Popup.PopupAnimationProperty.AddOwner(typeof(ContextMenu));
        }

        public ContextMenu()
        {
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (e.NewSource == null)
                return;
            Popup parent = LogicalTreeHelper.GetParent(this) as Popup;
            if (parent == null)
                return;
            Binding binding1 = new Binding {Source = this};
            PropertyPath propertyPath = new PropertyPath(PopupAnimationProperty);
            binding1.Path = propertyPath;
            Binding binding2 = binding1;
            parent.SetBinding(Popup.PopupAnimationProperty, binding2);
        }

        public PopupAnimation PopupAnimation
        {
            get => (PopupAnimation)GetValue(PopupAnimationProperty);
            set => SetValue(PopupAnimationProperty, value);
        }

        private void ChangeTheme(Theme oldValue, Theme newValue)
        {
            var oldTheme = oldValue;
            var newTheme = newValue;
            var resources = Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }

            if (newTheme != null)
            {
                resources.MergedDictionaries.Add(new ResourceDictionary {Source = newTheme.GetResourceUri()});
            }
        }
    }
}