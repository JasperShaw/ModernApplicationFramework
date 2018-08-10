using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public abstract class MarkerFormatDefinition : EditorFormatDefinition
    {
        public const string BorderId = "MarkerFormatDefinition/BorderId";
        public const string FillId = "MarkerFormatDefinition/FillId";
        public const string ZOrderId = "MarkerFormatDefinition/ZOrderId";

        protected Pen Border { get; set; }

        protected Brush Fill { get; set; }

        protected int ZOrder { get; set; }

        protected override ResourceDictionary CreateResourceDictionaryFromDefinition()
        {
            var dictionaryFromDefinition = base.CreateResourceDictionaryFromDefinition();
            if (Border != null)
                dictionaryFromDefinition["MarkerFormatDefinition/BorderId"] = Border;
            dictionaryFromDefinition["MarkerFormatDefinition/FillId"] = Fill;
            dictionaryFromDefinition["MarkerFormatDefinition/ZOrderId"] = ZOrder;
            return dictionaryFromDefinition;
        }
    }
}