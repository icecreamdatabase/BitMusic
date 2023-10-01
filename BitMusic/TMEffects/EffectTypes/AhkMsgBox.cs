using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMsgBox : EffectBase
{
    public string Text { get; }

    public AhkMsgBox(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string text) :
        base(settingsHandler, displayName, enabled, weight)
    {
        Text = text;
    }

    private protected override void ExecuteRaw()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}"
                        MsgBox, {{Text}}
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
