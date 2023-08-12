using System.Diagnostics;
using System.Threading.Tasks;
using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class ShowUnregisteredHypercam : EffectBase
{
    private readonly int _durationMs;

    public ShowUnregisteredHypercam(string displayName, bool enabled, uint weight, int durationMs) : base(displayName,
        enabled, weight)
    {
        _durationMs = durationMs;
    }

    public override void Execute(XmlTmSettings tmSettings)
    {
        Task.Run(() => ExecuteTask(_durationMs));
    }

    private static void ExecuteTask(int durationMs)
    {
        Stopwatch sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalMilliseconds < durationMs)
        {
            UnregisteredHypercam.ShowOverlay("Unregistered HyperCam 2");
            Task.Delay(1);
        }

        UnregisteredHypercam.ClearOverlay();
    }
}
