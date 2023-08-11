using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class PressTwoKeysWithDelay : PressKey
{
    public string AhkKeyCode2 { get; }
    public int DelayBetweenKeys { get; }

    public PressTwoKeysWithDelay(string displayName, bool enabled, uint weight, string ahkKeyCode, string ahkKeyCode2,
        int delayBetweenKeys) : base(displayName, enabled, weight,
        ahkKeyCode)
    {
        AhkKeyCode2 = ahkKeyCode2;
        DelayBetweenKeys = delayBetweenKeys;
    }

    public override void Execute(string processName)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}
                        Send , {{{AhkKeyCode}}}
                        Sleep, {{DelayBetweenKeys}}
                        Send , {{{AhkKeyCode2}}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
