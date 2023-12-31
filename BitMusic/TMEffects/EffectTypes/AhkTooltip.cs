﻿using System;
using AutoHotkey.Interop;
using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class AhkTooltip : EffectBase
{
    private Random _offsetRandom = new();

    public string Text { get; }
    public int TimeBetweenJumpsMs { get; }
    public int Jumps { get; }

    public AhkTooltip(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight, string text,
        int timeBetweenJumpsMs, int jumps) : base(settingsHandler, displayName, enabled, weight)
    {
        Text = text;
        TimeBetweenJumpsMs = timeBetweenJumpsMs;
        Jumps = jumps;
    }

    private protected override void ExecuteRaw()
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{TmSettings.ProcessName}}"
                        Loop, {{Jumps}} {
                            Random, xOffset, -300, 300
                            Random, yOffset, -300, 300
                            ToolTip, `n`n        {{Text}}        `n`n, A_ScreenWidth //2 + xOffset, A_ScreenHeight //2 + yOffset
                            Sleep, {{TimeBetweenJumpsMs}}
                        }
                        ToolTip
                        """;

        AutoHotkeyEngine.Instance.ExecRaw(code);
    }
}
