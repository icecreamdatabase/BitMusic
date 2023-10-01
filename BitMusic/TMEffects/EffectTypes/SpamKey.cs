using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class SpamKey : HoldKey
{
    public int HoldTimeMs { get; }
    public int ReleaseTimeMs { get; }

    public SpamKey(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string ahkKeyCode,
        int activeTimeMs, int holdTimeMs = 95, int releaseTimeMs = 5) :
        base(settingsHandler, displayName, enabled, weight, ahkKeyCode, activeTimeMs)
    {
        HoldTimeMs = holdTimeMs;
        ReleaseTimeMs = releaseTimeMs;
    }

    private protected override void ExecuteRaw()
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(HoldTimeMs + ReleaseTimeMs));

        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Loop, {{repeatCount}} {
                            Send , {{{AhkKeyCode}} down}
                            Sleep, {{HoldTimeMs}}
                            Send , {{{AhkKeyCode}} up}
                            Sleep, {{ReleaseTimeMs}}
                        }
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
