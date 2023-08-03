using System;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkMutespam : EffectBase
{
    public int ActiveTimeMs { get; }
    public int MuteDurationMs { get; }
    public int UnmuteDurationMs { get; }

    public AhkMutespam(int weight, int activeTimeMs, int muteDurationMs = 150, int unmuteDurationMs = 50) :
        base(weight)
    {
        ActiveTimeMs = activeTimeMs;
        MuteDurationMs = muteDurationMs;
        UnmuteDurationMs = unmuteDurationMs;
    }
    
    public override string GetConsoleOutput()
    {
        return $"🏎 Spamming mute for {Math.Round(ActiveTimeMs / 1000f, 1)} s";
    }

    public override void Execute(string processName)
    {
        int repeatCount = (int)(ActiveTimeMs / (float)(MuteDurationMs + UnmuteDurationMs));

        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}
                        Loop, {{repeatCount}} {
                            SoundSet, +1,, Mute
                            Sleep, {{MuteDurationMs}}
                            SoundSet, +1,, Mute
                            Sleep, {{UnmuteDurationMs}}
                        }
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
