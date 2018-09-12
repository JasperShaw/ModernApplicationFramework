using System;
using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.Native
{
    internal sealed class Ole2BclDataObject : IDataObject
    {
        private readonly IDataObject _bclData;

        internal Ole2BclDataObject(IDataObject bclData)
        {
            _bclData =
                bclData ?? throw new ArgumentNullException("System.Runtime.InteropServices.ComTypes.IDataObject");
        }

        void IDataObject.GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            if (_bclData != null)
            {
                _bclData.GetData(ref format, out medium);
            }
            else
                medium = default;
        }

        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            _bclData?.GetDataHere(ref format, ref medium);
        }

        public int QueryGetData(ref FORMATETC format)
        {
            if (_bclData != null)
                return _bclData.QueryGetData(ref format);
            return -1;
        }

        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            if (_bclData != null)
                return _bclData.GetCanonicalFormatEtc(ref formatIn, out formatOut);
            formatOut = default;
            return -1;
        }

        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            _bclData?.SetData(ref formatIn, ref medium, release);
        }

        public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
        {
            return _bclData?.EnumFormatEtc(direction);
        }

        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            if (_bclData != null)
                return _bclData.DAdvise(ref pFormatetc, advf, adviseSink, out connection);

            connection = 0;
            return -1;
        }

        public void DUnadvise(int connection)
        {
            _bclData?.DUnadvise(connection);
        }

        public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            if (_bclData != null)
                return _bclData.EnumDAdvise(out enumAdvise);
            enumAdvise = default;
            return -1;
        }
    }
}
