using System.Diagnostics;
using System.Threading.Tasks;
using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class ShowUnregisteredHypercam : EffectBase
{
    private readonly int _durationMs;

    public ShowUnregisteredHypercam(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight,
        int durationMs) : base(settingsHandler, displayName, enabled, weight)
    {
        _durationMs = durationMs;
    }

    private protected override void ExecuteRaw()
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            while (sw.Elapsed.TotalMilliseconds < _durationMs)
            {
                UnregisteredHypercam.ShowOverlay("Unregistered HyperCam 2");
                Task.Delay(1);
            }
        }
        finally
        {
            UnregisteredHypercam.ClearOverlay();
        }
    }
}
