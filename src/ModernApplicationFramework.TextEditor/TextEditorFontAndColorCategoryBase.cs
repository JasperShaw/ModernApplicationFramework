using System;

namespace ModernApplicationFramework.TextEditor
{
    internal abstract class TextEditorFontAndColorCategoryBase : FontAndColorCategoryBase
    {
        protected bool RaiseTextManagerEvents = true;
        private bool _hasFontChanged;
        //private IVsTextManager _textManager;

        //protected IVsTextManager TextManager
        //{
        //    get
        //    {
        //        if (this._textManager == null)
        //            this._textManager = Common.GetService<IVsTextManager, SVsTextManager>(this.ServiceProvider);
        //        return this._textManager;
        //    }
        //}

        protected bool LogEncodedColorData => false;

        public override int OnFontChanged(ref Guid rguidCategory, FontInfo[] pInfo, Logfont[] pLogfont, uint hfont)
        {
            _hasFontChanged = true;
            return base.OnFontChanged(ref rguidCategory, pInfo, pLogfont, hfont);
        }

        public override int OnApply()
        {
            try
            {
                int hr = base.OnApply();
                //TODO: TextManager Stuff
                //if (hr == 0 && _raiseTextManagerEvents)
                //{
                //    var textManager = TextManager as IVsPrivTextManager;
                //    if (textManager != null)
                //        hr = textManager.FireFontColorPreferencesChangedEvent(_hasFontChanged);
                //}
                return hr;
            }
            finally
            {
                _hasFontChanged = false;
            }
        }
    }
}