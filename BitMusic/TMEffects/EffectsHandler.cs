using System;
using System.Collections.Generic;
using System.Linq;
using BitMusic.TMEffects.EffectHelper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.TMEffects;

public static class EffectsHandler
{
    #region Effects

    public static readonly IReadOnlyList<EffectBase> Effects = new List<EffectBase>
    {
        new PressKey("Give up", true, 1, "Del"),
        new PressKey("Respawn", true, 1, "Backspace"),
        new PressKey("Cam 1", true, 1, "Numpad1"),
        new PressKey("Cam 2", true, 1, "Numpad2"),
        new PressKey("Cam 3", true, 1, "Numpad3"),
        new PressKey("Cam 7", true, 1, "Numpad7"),

        new HoldKey("Honk horn", true, 1, "Numpad0", 5000),
        new HoldKey("Action key 1", true, 1, "1", 5000),
        new HoldKey("Action key 2", true, 1, "2", 5000),
        new HoldKey("Action key 3", true, 1, "3", 5000),
        new HoldKey("Action key 4", true, 1, "4", 5000),
        new HoldKey("Action key 5", true, 1, "5", 5000),

        new SpamKey("Spam Cam 1", true, 1, "Numpad1", 5000),
        new SpamKey("Spam Cam 2", true, 1, "Numpad2", 5000),
        new SpamKey("Spam Cam 3", true, 1, "Numpad3", 5000),
        new SpamKey("Spam W", true, 1, "w", 2000),
        new SpamKey("Spam S", true, 1, "s", 2000),
        new SpamKey("Spam A", true, 1, "a", 2000),
        new SpamKey("Spam D", true, 1, "d", 2000),

        new SpamTwoKeys("Spam W S", true, 1, "w", "s", 2000),
        new SpamTwoKeys("Spam A D", true, 1, "a", "d", 2000),

        new DisplayRotationEffect("Rotate display 90°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw90, 5000),
        new DisplayRotationEffect("Rotate display 180°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw180, 5000),
        new DisplayRotationEffect("Rotate display 270°", true, 1, 1,
            DisplayRotationHelper.Orientations.DegreesCw270, 5000),

        new AhkMutespam("Spam mute", true, 1, 5000),
        new PressKey("Open Magnifier", true, 1, "LWin down}{NumpadAdd}{LWin up"),

        new AhkMsgBox("MsgBox", true, 1, "Hi :)"),
        new AhkTooltip("Tooltip", true, 1, "Hi :)", 400, 20),

        new ShowUnregisteredHypercam("Unregistered Hypercam 2", true, 1, 10_000),
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
