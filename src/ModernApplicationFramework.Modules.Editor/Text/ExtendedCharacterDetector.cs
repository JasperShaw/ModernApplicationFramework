using System.Text;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class ExtendedCharacterDetector : UTF8Encoding
    {
        internal bool DecodedExtendedCharacters { get; private set; }

        internal ExtendedCharacterDetector()
            : base(false, true)
        {
            DecodedExtendedCharacters = false;
        }

        public override Decoder GetDecoder()
        {
            return new ExtendedCharacterDetectionDecoder(base.GetDecoder(), HandleExtendedCharacter);
        }

        private void HandleExtendedCharacter()
        {
            DecodedExtendedCharacters = true;
        }
    }
}