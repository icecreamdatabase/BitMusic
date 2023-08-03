using System.Threading.Tasks;
using AutoHotkey.Interop;

namespace BitMusic.TMEffects.EffectHelper;

public static class AhkHelper
{
    public static void ExecuteAhkScript(string script) => Task.Run(() => AhkTask(script));

    private static void AhkTask(string code) => AutoHotkeyEngine.Instance.ExecRaw(code);
}
