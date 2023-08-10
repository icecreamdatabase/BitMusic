using System.Threading.Tasks;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class DisplayRotationEffect : EffectBase
{
    public uint DisplayNumber { get; }
    public DisplayRotationHelper.Orientations Orientation { get; }
    public int DurationMs { get; }

    public DisplayRotationEffect(string displayName, bool enabled, uint weight, uint displayNumber,
        DisplayRotationHelper.Orientations orientation, int durationMs) : base(displayName, enabled, weight)
    {
        DisplayNumber = displayNumber;
        Orientation = orientation;
        DurationMs = durationMs;
    }

    public override void Execute(string processName)
    {
        Task.Run(() => DisplayRotationTask(DisplayNumber, Orientation, DurationMs));
    }

    private static void DisplayRotationTask(uint displayNumber, DisplayRotationHelper.Orientations orientation,
        int durationMs)
    {
        try
        {
            DisplayRotationHelper.Rotate(displayNumber, orientation);
            Task.Delay(durationMs);
        }
        finally
        {
            DisplayRotationHelper.Rotate(displayNumber, DisplayRotationHelper.Orientations.DegreesCw0);
        }
    }
}
