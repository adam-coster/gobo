namespace Gobo.Printer.DocTypes;

internal sealed class HardLine : LineDoc, IBreakParent
{
    public bool SkipBreakIfFirstInGroup { get; }

    public HardLine(bool squash = false, bool skipBreakIfFirstInGroup = false)
    {
        Type = LineType.Hard;
        Squash = squash;
        SkipBreakIfFirstInGroup = skipBreakIfFirstInGroup;
    }
}
