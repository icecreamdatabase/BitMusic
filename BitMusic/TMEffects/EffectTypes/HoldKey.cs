using System;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class HoldKey : PressKey
{
    public int ActiveTimeMs { get; }

    public HoldKey(int weight, string ahkKeyCode, int activeTimeMs) : base(weight, ahkKeyCode)
    {
        ActiveTimeMs = activeTimeMs;
    }

    public override string GetConsoleOutput()
    {
        return $"🏎 Holding {AhkKeyCode} for {Math.Round(ActiveTimeMs / 1000f, 1)} s";
    }

    public override void Execute(string processName)
    {
        string code =$$"""
                 IfWinActive ahk_exe {{processName}}"
                 Send , {{{AhkKeyCode}} down}"
                 Sleep, {{{ActiveTimeMs}}}"
                 Send , {{{AhkKeyCode}} up}"
                 """;
        
        AhkHelper.ExecuteAhkScript(code);
    }
}
