using System;
using System.Threading.Tasks;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class DisplayRotationEffect : EffectBase
{
    public uint DisplayNumber { get; }
    public DisplayRotationHelper.Orientations Orientation { get; }
    public int DurationMs { get; }

    public DisplayRotationEffect(int weight, uint displayNumber, DisplayRotationHelper.Orientations orientation, int durationMs) : base(weight)
    {
        DisplayNumber = displayNumber;
        Orientation = orientation;
        DurationMs = durationMs;
    }

    public override string GetConsoleOutput()
    {
        return $"💻 Rotating the main monitor for {Math.Round(DurationMs / 1000f, 1)} s";
    }

    public override void Execute(string processName)
    {
        Task.Run(() => DisplayRotationTask(DisplayNumber, Orientation, DurationMs));
    }

    private static void DisplayRotationTask(uint displayNumber, DisplayRotationHelper.Orientations orientation, int durationMs)
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
