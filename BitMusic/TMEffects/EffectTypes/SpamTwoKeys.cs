using System;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class SpamTwoKeys : SpamKey
{
    public string AhkKeyCode2 { get; }

    public SpamTwoKeys(int weight, string ahkKeyCode, string ahkKeyCode2, int activeTimeMs,
        int holdTimeMs = 50, int releaseTimeMs = 50) :
        base(weight, ahkKeyCode, activeTimeMs, holdTimeMs, releaseTimeMs)
    {
        AhkKeyCode2 = ahkKeyCode2;
    }

    public override string GetConsoleOutput()
    {
        return $"🏎 Spamming {AhkKeyCode} and {AhkKeyCode2} for {Math.Round(ActiveTimeMs / 1000f, 1)} s";
    }

    public override void Execute(string processName)
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(HoldTimeMs + ReleaseTimeMs));

        string code = $$"""
                        IfWinActive ahk_exe {{processName}}"
                        Loop, {{{repeatCount}}} {
                            Send , {{{AhkKeyCode}} down}"
                            Sleep, {{{HoldTimeMs}}}"
                            Send , {{{AhkKeyCode}} up}"
                            Sleep, {{{ReleaseTimeMs}}}"
                            Send , {{{AhkKeyCode2}} down}"
                            Sleep, {{{HoldTimeMs}}}"
                            Send , {{{AhkKeyCode2}} up}"
                            Sleep, {{{ReleaseTimeMs}}}"
                        }
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
