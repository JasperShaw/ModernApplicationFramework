﻿using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Editor.OutputClassifier
{
    internal class PendingOutput
    {
        public string Message { get; }

        public IClassificationType Classification { get; }

        public PendingOutput(string message, IClassificationType classification)
        {
            Message = message;
            Classification = classification;
        }
    }
}