namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal static class TextModelUtilities
    {
        public static int ComputeLineCountDelta(LineBreakBoundaryConditions boundaryConditions, StringRebuilder oldText,
            StringRebuilder newText)
        {
            var num = -oldText.LineBreakCount + newText.LineBreakCount;
            if ((boundaryConditions & LineBreakBoundaryConditions.PrecedingReturn) != LineBreakBoundaryConditions.None)
            {
                if (oldText.FirstCharacter == '\n')
                    ++num;
                if (newText.FirstCharacter == '\n')
                    --num;
            }

            if ((boundaryConditions & LineBreakBoundaryConditions.SucceedingNewline) !=
                LineBreakBoundaryConditions.None)
            {
                if (oldText.LastCharacter == '\r')
                    ++num;
                if (newText.LastCharacter == '\r')
                    --num;
            }

            if (oldText.Length == 0 &&
                (boundaryConditions & LineBreakBoundaryConditions.PrecedingReturn) !=
                LineBreakBoundaryConditions.None &&
                (boundaryConditions & LineBreakBoundaryConditions.SucceedingNewline) !=
                LineBreakBoundaryConditions.None)
                ++num;
            if (newText.Length == 0 &&
                (boundaryConditions & LineBreakBoundaryConditions.PrecedingReturn) !=
                LineBreakBoundaryConditions.None &&
                (boundaryConditions & LineBreakBoundaryConditions.SucceedingNewline) !=
                LineBreakBoundaryConditions.None)
                --num;
            return num;
        }
    }
}