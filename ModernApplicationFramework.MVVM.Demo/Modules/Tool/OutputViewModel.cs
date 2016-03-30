﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.MVVM.Core;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Tool
{
    [Export(typeof(IOutput))]
    public sealed class OutputViewModel: Controls.Tool, IOutput
    {
        private readonly StringBuilder _stringBuilder;
        private readonly OutputWriter _writer;
        private IOutputView _view;

        public override PaneLocation PreferredLocation => PaneLocation.Bottom;

        public TextWriter Writer => _writer;

        public OutputViewModel()
        {
            DisplayName = "Output";
            _stringBuilder = new StringBuilder();
            _writer = new OutputWriter(this);
        }

        public void Clear()
        {
            if (_view != null)
                Execute.OnUiThread(() => _view.Clear());
            _stringBuilder.Clear();
        }

        public void AppendLine(string text)
        {
            Append(text + Environment.NewLine);
        }

        public void Append(string text)
        {
            _stringBuilder.Append(text);
            OnTextChanged();
        }

        private void OnTextChanged()
        {
            if (_view != null)
                Execute.OnUiThread(() => _view.SetText(_stringBuilder.ToString()));
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (IOutputView)view;
            _view.SetText(_stringBuilder.ToString());
            _view.ScrollToEnd();
        }
    }
}
