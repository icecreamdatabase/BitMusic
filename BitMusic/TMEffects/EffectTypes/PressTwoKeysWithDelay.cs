using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class PressTwoKeysWithDelay : PressKey
{
    public string AhkKeyCode2 { get; }
    public int DelayBetweenKeys { get; }

    public PressTwoKeysWithDelay(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight,
        string ahkKeyCode, string ahkKeyCode2, int delayBetweenKeys) :
        base(settingsHandler, displayName, enabled, weight, ahkKeyCode)
    {
        AhkKeyCode2 = ahkKeyCode2;
        DelayBetweenKeys = delayBetweenKeys;
    }

    private protected override void ExecuteRaw()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}}}
                        Sleep, {{DelayBetweenKeys}}
                        Send , {{{AhkKeyCode2}}}
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
