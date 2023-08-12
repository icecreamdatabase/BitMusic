using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMsgBox : EffectBase
{
    public string Text { get; }

    public AhkMsgBox(string displayName, bool enabled, uint weight, string text) : base(displayName, enabled, weight)
    {
        Text = text;
    }

    public override void Execute(XmlTmSettings tmSettings)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{tmSettings.ProcessName}}"
                        MsgBox, {{Text}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
