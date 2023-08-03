using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class PressKey : EffectBase
{
    public string AhkKeyCode { get; }

    public PressKey(int weight, string ahkKeyCode) : base(weight)
    {
        AhkKeyCode = ahkKeyCode;
    }

    public override string GetConsoleOutput()
    {
        return $"🏎 Pressing {AhkKeyCode}";
    }

    public override void Execute(string processName)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}
                        Send , {{{AhkKeyCode}}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
