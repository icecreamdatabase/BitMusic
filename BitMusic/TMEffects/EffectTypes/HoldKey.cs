using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class HoldKey : PressKey
{
    public int ActiveTimeMs { get; }

    public HoldKey(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string ahkKeyCode,
        int activeTimeMs) : base(settingsHandler, displayName, enabled, weight, ahkKeyCode)
    {
        ActiveTimeMs = activeTimeMs;
    }

    private protected override void ExecuteRaw()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}} down}
                        Sleep, {{ActiveTimeMs}}
                        Send , {{{AhkKeyCode}} up}
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
