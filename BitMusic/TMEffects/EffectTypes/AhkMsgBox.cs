using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMsgBox : EffectBase
{
    public string Text { get; }

    public AhkMsgBox(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string text) :
        base(settingsHandler, displayName, enabled, weight)
    {
        Text = text;
    }

    public override void Execute()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}"
                        MsgBox, {{Text}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
