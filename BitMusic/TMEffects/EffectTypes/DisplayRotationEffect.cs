using System.Threading.Tasks;
using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class DisplayRotationEffect : EffectBase
{
    public DisplayRotationHelper.Orientations Orientation { get; }
    public int DurationMs { get; }

    public DisplayRotationEffect(string displayName, bool enabled, uint weight,
        DisplayRotationHelper.Orientations orientation, int durationMs) : base(displayName, enabled, weight)
    {
        Orientation = orientation;
        DurationMs = durationMs;
    }

    public override void Execute(XmlTmSettings tmSettings)
    {
        Task.Run(() => DisplayRotationTask(tmSettings.MainDisplayNumber, Orientation, DurationMs));
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
