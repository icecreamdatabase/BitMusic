using System;
using System.Collections.Generic;
using System.Linq;
using BitMusic.TMEffects.EffectHelper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.TMEffects;

public static class EffectsHandler
{
    #region Effects

    private static readonly IReadOnlyList<EffectBase> Effects = new List<EffectBase>
    {
        new PressKey(1, "Del"),
        new PressKey(1, "Backspace"),
        new PressKey(1, "Numpad1"),
        new PressKey(1, "Numpad2"),
        new PressKey(1, "Numpad3"),
        new PressKey(1, "Numpad7"),

        new HoldKey(1, "Numpad0", 5000),

        new SpamKey(1, "Numpad1", 5000),
        new SpamKey(1, "Numpad2", 5000),
        new SpamKey(1, "Numpad3", 5000),
        new SpamKey(1, "w", 2000),
        new SpamKey(1, "s", 2000),
        new SpamKey(1, "a", 2000),
        new SpamKey(1, "d", 2000),

        new SpamTwoKeys(1, "w", "s", 2000),
        new SpamTwoKeys(1, "a", "d", 2000),

        new DisplayRotationEffect(1, 1, DisplayRotationHelper.Orientations.DegreesCw90, 5000),
        new DisplayRotationEffect(1, 1, DisplayRotationHelper.Orientations.DegreesCw180, 5000),
        new DisplayRotationEffect(1, 1, DisplayRotationHelper.Orientations.DegreesCw270, 5000),

        new AhkMutespam(1, 5000),
        new PressKey(1, "LWin down}{NumpadAdd}{LWin up"), // Open screen magnifier

        new AhkMsgBox(1, "Hi :)"),
        new AhkTooltip(1, "Hi :)", 400, 20),
    };

    #endregion

    #region Properties and Methods

    private static readonly Random WeightRandom = new();

    private static int TotalWeight => Effects.Sum(effect => effect.Weight);

    /// <summary>
    /// Weighted random selection<br/>
    /// https://www.educative.io/answers/what-is-the-weighted-random-selection-algorithm <br/>
    /// https://stackoverflow.com/questions/56692/random-weighted-choice
    /// </summary>
    /// <returns></returns>
    private static EffectBase? SelectRandomEffectByWeight()
    {
        int randomIndexForWeightCalc = WeightRandom.Next(0, TotalWeight);

        foreach (EffectBase effect in Effects)
        {
            if (randomIndexForWeightCalc < effect.Weight)
                return effect;

            randomIndexForWeightCalc -= effect.Weight;
        }

        // No effect found with that weight or no effects are defined in the first place.
        return null;
    }

    public static EffectBase? ExecuteRandomEffectByWeight(string processName)
    {
        EffectBase? randomEffect = SelectRandomEffectByWeight();

        // Do nothing if have no effects
        if (randomEffect == null)
            return null;

        randomEffect.Execute(processName);
        return randomEffect;
    }

    #endregion
}
