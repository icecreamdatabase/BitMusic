using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMsgBox : EffectBase
{
    public string Text { get; }

    public AhkMsgBox(string displayName, bool enabled, int weight, string text) : base(displayName, enabled, weight)
    {
        Text = text;
    }

    public override void Execute(string processName)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}"
                        MsgBox, {{Text}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
