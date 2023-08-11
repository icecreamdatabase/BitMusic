﻿using System;
using System.Collections.Generic;
using System.Linq;
using BitMusic.TMEffects.EffectHelper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.TMEffects;

public static class EffectsHandler
{
    #region Effects

    public static readonly IReadOnlyList<EffectBase> DefaultEffects = new List<EffectBase>
    {
        new PressKey("Give up", true, 1, "Del"),
        new PressKey("Respawn", true, 1, "Backspace"),
        new PressKey("Switch to cam 1", true, 1, "Numpad1"),
        new PressKey("Switch to cam 2", true, 1, "Numpad2"),
        new PressKey("Switch to cam 3", true, 1, "Numpad3"),
        new PressKey("Switch to cam 7", true, 1, "Numpad7"),

        new PressTwoKeysWithDelay("Temporarily switch to cam 2", true, 1, "Numpad2", "Numpad1", 5000),
        new PressTwoKeysWithDelay("Temporarily switch to cam 3", true, 1, "Numpad3", "Numpad1", 5000),
        new PressTwoKeysWithDelay("Temporarily switch to cam 7", true, 1, "Numpad7", "Numpad1", 5000),

        new HoldKey("Honk horn", true, 1, "Numpad0", 5000),
        new HoldKey("Hold action key 1", true, 1, "1", 5000),
        new HoldKey("Hold action key 2", true, 1, "2", 5000),
        new HoldKey("Hold action key 3", true, 1, "3", 5000),
        new HoldKey("Hold action key 4", true, 1, "4", 5000),
        new HoldKey("Hold action key 5", true, 1, "5", 5000),

        new SpamKey("Spam Cam 1", true, 1, "Numpad1", 5000),
        new SpamKey("Spam Cam 2", true, 1, "Numpad2", 5000),
        new SpamKey("Spam Cam 3", true, 1, "Numpad3", 5000),
        new SpamKey("Spam W", true, 1, "w", 2000),
        new SpamKey("Spam S", true, 1, "s", 2000),
        new SpamKey("Spam A", true, 1, "a", 2000),
        new SpamKey("Spam D", true, 1, "d", 2000),

        new SpamTwoKeys("Spam W S", true, 1, "w", "s", 2000),
        new SpamTwoKeys("Spam A D", true, 1, "a", "d", 2000),

        new DisplayRotationEffect("Rotate display to 90°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw90, 5000),
        new DisplayRotationEffect("Rotate display to 180°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw180, 5000),
        new DisplayRotationEffect("Rotate display to 270°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw270, 5000),

        new AhkMutespam("Spam system mute", true, 1, 5000),
        new PressKey("Open Windows Magnifier", true, 1, "LWin down}{NumpadAdd}{LWin up"),

        new AhkMsgBox("Show MessageBox", true, 1, "Hi :)"),
        new AhkTooltip("Show many Tooltips", true, 1, "Hi :)", 400, 20),

        new ShowUnregisteredHypercam("\"Unregistered Hypercam 2\" watermark", true, 1, 10_000),
    };

    #endregion

    #region Properties and Methods

    private static readonly Random WeightRandom = new();

    private static long TotalWeight(this IEnumerable<EffectBase> effects) =>
        effects.Where(effect => effect.Enabled).Sum(effect => effect.Weight);

    /// <summary>
    /// Weighted random selection<br/>
    /// https://www.educative.io/answers/what-is-the-weighted-random-selection-algorithm <br/>
    /// https://stackoverflow.com/questions/56692/random-weighted-choice
    /// </summary>
    /// <returns></returns>
    private static EffectBase? SelectRandomEffectByWeight(this ICollection<EffectBase> effects)
    {
        long randomIndexForWeightCalc = WeightRandom.NextInt64(0, TotalWeight(effects));

        foreach (EffectBase effect in effects.Where(effect => effect.Enabled))
        {
            if (randomIndexForWeightCalc < effect.Weight)
                return effect;

            randomIndexForWeightCalc -= effect.Weight;
        }

        // No effect found with that weight or no effects are defined in the first place.
        return null;
    }

    public static EffectBase? ExecuteRandomEffectByWeight(this ICollection<EffectBase> effects, string processName)
    {
        EffectBase? randomEffect = SelectRandomEffectByWeight(effects);

        // Do nothing if have no effects
        if (randomEffect == null)
            return null;

        randomEffect.Execute(processName);
        return randomEffect;
    }

    #endregion
}
