using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class ShowUnregisteredHypercam : EffectBase
{
    private readonly int _durationMs;

    public ShowUnregisteredHypercam(string displayName, bool enabled, int weight, int durationMs) : base(displayName,
        enabled, weight)
    {
        _durationMs = durationMs;
    }

    public override string GetConsoleOutput()
    {
        return $"🎥 Showing \"Unregistered Hypercam 2\" overlay for {Math.Round(_durationMs / 1000f, 1)}";
    }

    public override void Execute(string processName)
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
