using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMutespam : EffectBase
{
    public int ActiveTimeMs { get; }
    public int MuteDurationMs { get; }
    public int UnmuteDurationMs { get; }

    public AhkMutespam(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, int activeTimeMs,
        int muteDurationMs = 150, int unmuteDurationMs = 50) :
        base(settingsHandler, displayName, enabled, weight)
    {
        ActiveTimeMs = activeTimeMs;
        MuteDurationMs = muteDurationMs;
        UnmuteDurationMs = unmuteDurationMs;
    }

    private protected override void ExecuteRaw()
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(MuteDurationMs + UnmuteDurationMs));

        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Loop, {{repeatCount}} {
                            SoundSet, +1,, Mute
                            Sleep, {{MuteDurationMs}}
                            SoundSet, +1,, Mute
                            Sleep, {{UnmuteDurationMs}}
                        }
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
