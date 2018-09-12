using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace ModernApplicationFramework.Native
{
    public class OleDataObject : DataObject, IDataObject
    {
        private readonly IDataObject _oleData;


        public OleDataObject(System.Windows.Forms.IDataObject winData)
            : base(winData)
        {
            _oleData = winData as IDataObject;
            if (_oleData != null)
                return;
            _oleData = new Ole2BclDataObject(this);
        }

        public OleDataObject(IDataObject comData)
            : base(comData)
        {
            _oleData = comData;
            if (_oleData != null)
                return;
            _oleData = new Ole2BclDataObject(comData);
        }

        int IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection)
        {
            return _oleData.DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);
        }

        void IDataObject.DUnadvise(int connection)
        {
            _oleData.DUnadvise(connection);
        }

        int IDataObject.EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            return _oleData.EnumDAdvise(out enumAdvise);
        }

        IEnumFORMATETC IDataObject.EnumFormatEtc(DATADIR direction)
        {
            return _oleData.EnumFormatEtc(direction);
        }

        int IDataObject.GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            return _oleData.GetCanonicalFormatEtc(ref formatIn, out formatOut);
        }

        void IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            _oleData.GetData(ref format, out medium);
        }

        void IDataObject.GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            _oleData.GetDataHere(ref format, ref medium);
        }

        int IDataObject.QueryGetData(ref FORMATETC format)
        {
            return _oleData.QueryGetData(ref format);
        }

        void IDataObject.SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, [MarshalAs(UnmanagedType.Bool)] bool release)
        {
            _oleData.SetData(ref formatIn, ref medium, release);
        }
    }
}
