﻿using System;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkTooltip : EffectBase
{
    private Random _offsetRandom = new();

    public string Text { get; }
    public int TimeBetweenJumpsMs { get; }
    public int Jumps { get; }

    public AhkTooltip(string displayName, bool enabled, int weight, string text, int timeBetweenJumpsMs, int jumps) :
        base(displayName, enabled, weight)
    {
        Text = text;
        TimeBetweenJumpsMs = timeBetweenJumpsMs;
        Jumps = jumps;
    }

    public override void Execute(string processName)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{processName}}"
                        Loop, {{Jumps}} {
                            Random, xOffset, -300, 300
                            Random, yOffset, -300, 300
                            ToolTip, `n`n        {{Text}}        `n`n, A_ScreenWidth //2 + xOffset, A_ScreenHeight //2 + yOffset
                            Sleep, {{TimeBetweenJumpsMs}}
                        }
                        ToolTip
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
