using System.IO;
using System.Text;

namespace ModernApplicationFramework.Text.Data
{
    public interface IEncodingDetector
    {
        Encoding GetStreamEncoding(Stream stream);
    }
}