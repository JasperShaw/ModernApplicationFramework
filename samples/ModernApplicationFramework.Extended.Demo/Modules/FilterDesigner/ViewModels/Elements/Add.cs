using System.Windows.Media;
using System.Windows.Media.Effects;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ShaderEffects;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ViewModels.Elements
{
    [ToolboxItemData("Add", "00000000-0000-0000-0000-000000000000", 0 ,true, typeof(GraphViewModel))]
    public class Add : ShaderEffectElement
    {
        protected override Effect GetEffect()
        {
            return new AddEffect
            {
                Input1 = new ImageBrush(InputConnectors[0].Value),
                Input2 = new ImageBrush(InputConnectors[1].Value)
            };
        }

        public Add()
        {
            AddInputConnector("Left", Colors.DarkSeaGreen);
            AddInputConnector("Right", Colors.DarkSeaGreen);
        }
    }
}
