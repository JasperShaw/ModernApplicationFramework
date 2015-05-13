using System.Windows;

namespace ModernApplicationFramework.Controls
{
    [TemplatePart( Name = PartActionButton, Type = typeof(System.Windows.Controls.Button))]
    public class SplittButton : DropDownButton
    {
        private const string PartActionButton = "PART_ActionButton";

        static SplittButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplittButton), new FrameworkPropertyMetadata(typeof(SplittButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button = GetTemplateChild(PartActionButton) as System.Windows.Controls.Button;
        }
    }

}
