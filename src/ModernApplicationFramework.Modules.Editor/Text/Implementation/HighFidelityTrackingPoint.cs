using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
{
    internal class HighFidelityTrackingPoint : TrackingPoint
    {
        private VersionPositionHistory _cachedPosition;
        private readonly TrackingFidelityMode _fidelity;

        internal HighFidelityTrackingPoint(ITextVersion version, int position, PointTrackingMode trackingMode, TrackingFidelityMode fidelity)
            : base(version, position, trackingMode)
        {
            if (fidelity != TrackingFidelityMode.UndoRedo && fidelity != TrackingFidelityMode.Backward)
                throw new ArgumentOutOfRangeException(nameof(fidelity));
            var noninvertibleHistory = (List<VersionNumberPosition>)null;
            if (fidelity == TrackingFidelityMode.UndoRedo && version.VersionNumber > 0)
            {
                noninvertibleHistory = new List<VersionNumberPosition>();
                if (version.VersionNumber != version.ReiteratedVersionNumber)
                    noninvertibleHistory.Add(new VersionNumberPosition(version.ReiteratedVersionNumber, position));
                noninvertibleHistory.Add(new VersionNumberPosition(version.VersionNumber, position));
            }
            _cachedPosition = new VersionPositionHistory(version, position, noninvertibleHistory);
            _fidelity = fidelity;
        }

        public override ITextBuffer TextBuffer => _cachedPosition.Version.TextBuffer;

        public override TrackingFidelityMode TrackingFidelity => _fidelity;

        protected override int TrackPosition(ITextVersion targetVersion)
        {
            var cachedPosition = _cachedPosition;
            if (targetVersion == cachedPosition.Version)
                return cachedPosition.Position;
            var noninvertibleHistory = cachedPosition.NoninvertibleHistory;
            int position;
            if (targetVersion.VersionNumber > cachedPosition.Version.VersionNumber)
            {
                position = TrackPositionForwardInTime(TrackingMode, _fidelity, ref noninvertibleHistory, cachedPosition.Position, cachedPosition.Version, targetVersion);
                _cachedPosition = new VersionPositionHistory(targetVersion, position, noninvertibleHistory);
            }
            else
                position = TrackPositionBackwardInTime(TrackingMode, _fidelity == TrackingFidelityMode.Backward ? noninvertibleHistory : null, cachedPosition.Position, cachedPosition.Version, targetVersion);
            return position;
        }

        internal static int TrackPositionForwardInTime(PointTrackingMode trackingMode, TrackingFidelityMode fidelity, ref List<VersionNumberPosition> noninvertibleHistory, int currentPosition, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            var textVersion = currentVersion;
            while (textVersion != targetVersion)
            {
                currentVersion = textVersion;
                textVersion = currentVersion.Next;
                if (fidelity == TrackingFidelityMode.UndoRedo && textVersion.ReiteratedVersionNumber != textVersion.VersionNumber && noninvertibleHistory != null)
                {
                    var index = noninvertibleHistory.BinarySearch(new VersionNumberPosition(textVersion.ReiteratedVersionNumber, 0), VersionNumberPositionComparer.Instance);
                    if (index >= 0)
                    {
                        currentPosition = noninvertibleHistory[index].Position;
                        continue;
                    }
                }
                var count = currentVersion.Changes.Count;
                if (count == 0)
                {
                    if (textVersion.VersionNumber != textVersion.ReiteratedVersionNumber)
                        RecordNoninvertibleTransition(ref noninvertibleHistory, currentPosition, textVersion.ReiteratedVersionNumber);
                }
                else
                {
                    var currentPosition1 = currentPosition;
                    for (var index = 0; index < count; ++index)
                    {
                        var change = currentVersion.Changes[index];
                        if (IsOpaque(change))
                        {
                            if (change.NewPosition + change.OldLength <= currentPosition)
                            {
                                currentPosition += change.Delta;
                            }
                            else
                            {
                                if (change.NewPosition <= currentPosition && change.NewEnd <= currentPosition)
                                {
                                    RecordNoninvertibleTransition(ref noninvertibleHistory, currentPosition1, currentVersion.VersionNumber);
                                    currentPosition = change.NewEnd;
                                }
                                break;
                            }
                        }
                        else if (trackingMode == PointTrackingMode.Positive)
                        {
                            if (change.NewPosition <= currentPosition)
                            {
                                if (change.NewPosition + change.OldLength <= currentPosition)
                                {
                                    currentPosition += change.Delta;
                                }
                                else
                                {
                                    RecordNoninvertibleTransition(ref noninvertibleHistory, currentPosition1, currentVersion.VersionNumber);
                                    currentPosition = change.NewEnd;
                                    break;
                                }
                            }
                            else
                                break;
                        }
                        else if (change.NewPosition < currentPosition)
                        {
                            if (change.NewPosition + change.OldLength < currentPosition)
                            {
                                currentPosition += change.Delta;
                            }
                            else
                            {
                                RecordNoninvertibleTransition(ref noninvertibleHistory, currentPosition1, currentVersion.VersionNumber);
                                currentPosition = change.NewPosition;
                                break;
                            }
                        }
                        else
                            break;
                    }
                }
            }
            return currentPosition;
        }

        private static void RecordNoninvertibleTransition(ref List<VersionNumberPosition> noninvertibleHistory, int currentPosition, int versionNumber)
        {
            if (noninvertibleHistory == null)
                noninvertibleHistory = new List<VersionNumberPosition>();
            if (noninvertibleHistory.Count > 0 && noninvertibleHistory[noninvertibleHistory.Count - 1].VersionNumber == versionNumber)
                return;
            noninvertibleHistory.Add(new VersionNumberPosition(versionNumber, currentPosition));
        }

        internal static int TrackPositionBackwardInTime(PointTrackingMode trackingMode, List<VersionNumberPosition> noninvertibleHistory, int currentPosition, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            var textVersionArray = new ITextVersion[currentVersion.VersionNumber - targetVersion.VersionNumber];
            var num = 0;
            for (var textVersion = targetVersion; textVersion != currentVersion; textVersion = textVersion.Next)
                textVersionArray[num++] = textVersion;
            while (num > 0)
            {
                var textVersion = textVersionArray[--num];
                if (noninvertibleHistory != null)
                {
                    var index = noninvertibleHistory.BinarySearch(new VersionNumberPosition(textVersion.VersionNumber, 0), VersionNumberPositionComparer.Instance);
                    if (index >= 0)
                    {
                        currentPosition = noninvertibleHistory[index].Position;
                        continue;
                    }
                }
                IList<ITextChange> changes = textVersion.Changes;
                for (var index = changes.Count - 1; index >= 0; --index)
                {
                    var textChange = changes[index];
                    if (IsOpaque(textChange))
                    {
                        if (textChange.NewEnd <= currentPosition)
                            currentPosition -= textChange.Delta;
                        else if (textChange.NewPosition <= currentPosition)
                            currentPosition = Math.Min(currentPosition, textChange.NewPosition + textChange.OldLength);
                    }
                    else if (trackingMode == PointTrackingMode.Positive)
                    {
                        if (textChange.NewPosition <= currentPosition)
                        {
                            if (textChange.NewEnd <= currentPosition)
                                currentPosition -= textChange.Delta;
                            else
                                currentPosition = textChange.NewPosition + textChange.OldLength;
                        }
                    }
                    else if (textChange.NewPosition < currentPosition)
                    {
                        if (textChange.NewEnd < currentPosition)
                            currentPosition -= textChange.Delta;
                        else
                            currentPosition = textChange.NewPosition;
                    }
                }
            }
            return currentPosition;
        }

        private static bool IsOpaque(ITextChange textChange)
        {
            if (textChange is ITextChange2 textChange2)
                return textChange2.IsOpaque;
            return false;
        }

        public override string ToString()
        {
            var cachedPosition = _cachedPosition;
            var stringBuilder = new StringBuilder("*");
            stringBuilder.Append(ToString(cachedPosition.Version, cachedPosition.Position, TrackingMode));
            if (cachedPosition.NoninvertibleHistory != null)
            {
                stringBuilder.Append("[");
                foreach (var versionNumberPosition in cachedPosition.NoninvertibleHistory)
                    stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "V{0}@{1}", versionNumberPosition.VersionNumber, versionNumberPosition.Position));
                stringBuilder.Append("]");
            }
            return stringBuilder.ToString();
        }

        private class VersionPositionHistory
        {
            public VersionPositionHistory(ITextVersion version, int position, List<VersionNumberPosition> noninvertibleHistory)
            {
                Version = version;
                Position = position;
                NoninvertibleHistory = noninvertibleHistory;
            }

            public ITextVersion Version { get; }

            public int Position { get; }

            public List<VersionNumberPosition> NoninvertibleHistory { get; }
        }
    }
}