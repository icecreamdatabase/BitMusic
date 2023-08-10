using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class SpamKey : HoldKey
{
    public int HoldTimeMs { get; }
    public int ReleaseTimeMs { get; }

    public SpamKey(string displayName, bool enabled, int weight, string ahkKeyCode, int activeTimeMs, int holdTimeMs = 95, int releaseTimeMs = 5) :
        base(displayName, enabled, weight, ahkKeyCode, activeTimeMs)
    {
        HoldTimeMs = holdTimeMs;
        ReleaseTimeMs = releaseTimeMs;
    }

    public override void Execute(string processName)
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(HoldTimeMs + ReleaseTimeMs));

        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}
                        Loop, {{repeatCount}} {
                            Send , {{{AhkKeyCode}} down}
                            Sleep, {{HoldTimeMs}}
                            Send , {{{AhkKeyCode}} up}
                            Sleep, {{ReleaseTimeMs}}
                        }
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
