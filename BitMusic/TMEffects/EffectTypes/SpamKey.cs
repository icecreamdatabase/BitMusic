using System;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class SpamKey : HoldKey
{
    public int HoldTimeMs { get; }
    public int ReleaseTimeMs { get; }

    public SpamKey(int weight, string ahkKeyCode, int activeTimeMs, int holdTimeMs = 95, int releaseTimeMs = 5) :
        base(weight, ahkKeyCode, activeTimeMs)
    {
        HoldTimeMs = holdTimeMs;
        ReleaseTimeMs = releaseTimeMs;
    }

    public override string GetConsoleOutput()
    {
        return $"🏎 Spamming {AhkKeyCode} for {Math.Round(ActiveTimeMs / 1000f, 1)} s";
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
