using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMsgBox : EffectBase
{
    public string Text { get; }

    public AhkMsgBox(int weight, string text) : base(weight)
    {
        Text = text;
    }

    public override string GetConsoleOutput()
    {
        return "📓 Showing message box";
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
