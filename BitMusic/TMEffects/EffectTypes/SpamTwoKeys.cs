using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class SpamTwoKeys : HoldKey
{
    public string AhkKeyCode2 { get; }
    public int HoldTimeMs { get; }
    public int HoldTimeMsAhkKeyCode2 { get; }

    public SpamTwoKeys(string displayName, bool enabled, uint weight, string ahkKeyCode, string ahkKeyCode2,
        int activeTimeMs, int holdTimeMs = 50, int holdTimeMsAhkKeyCode2 = 50) : base(displayName, enabled, weight,
        ahkKeyCode, activeTimeMs)
    {
        AhkKeyCode2 = ahkKeyCode2;
        HoldTimeMs = holdTimeMs;
        HoldTimeMsAhkKeyCode2 = holdTimeMsAhkKeyCode2;
    }

    public override void Execute(string processName)
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(HoldTimeMs + HoldTimeMsAhkKeyCode2 + 5 + 5));

        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}
                        Loop, {{repeatCount}} {
                            Send , {{{AhkKeyCode}} down}
                            Sleep, {{HoldTimeMs}}
                            Send , {{{AhkKeyCode}} up}
                            Send , {{{AhkKeyCode2}} down}
                            Sleep, {{HoldTimeMsAhkKeyCode2}}
                            Send , {{{AhkKeyCode2}} up}
                        }
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
