﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BitMusic.FileWriters;
using BitMusic.Helper;
using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.TMEffects;

public class EffectsHandler
{
    #region Effects

    private static IEnumerable<EffectBase> GetDefaultEffects(SettingsHandler s) => new List<EffectBase>
    {
        //@formatter:off
        new PressKey(s, "Give up", true, 1, "Del"),
        new PressKey(s, "Respawn", true, 1, "Backspace"),
        new PressKey(s, "Switch to cam 1", true, 1, "Numpad1"),
        new PressKey(s, "Switch to cam 2", true, 1, "Numpad2"),
        new PressKey(s, "Switch to cam 3", true, 1, "Numpad3"),
        new PressKey(s, "Switch to cam 7", true, 1, "Numpad7"),
        new PressKey(s, "Next Song", true, 1, "Media_Next"),

        new PressTwoKeysWithDelay(s, "Temporarily switch to cam 2", true, 1, "Numpad2", "Numpad1", 5000),
        new PressTwoKeysWithDelay(s, "Temporarily switch to cam 3", true, 1, "Numpad3", "Numpad1", 5000),
        new PressTwoKeysWithDelay(s, "Temporarily switch to cam 7", true, 1, "Numpad7", "Numpad1", 5000),

        new HoldKey(s, "Honk horn", true, 1, "Numpad0", 5_000),
        new HoldKey(s, "Hold action key 1", true, 1, "1", 5_000),
        new HoldKey(s, "Hold action key 2", true, 1, "2", 5_000),
        new HoldKey(s, "Hold action key 3", true, 1, "3", 5_000),
        new HoldKey(s, "Hold action key 4", true, 1, "4", 5_000),
        new HoldKey(s, "Hold action key 5", true, 1, "5", 5_000),

        new SpamKey(s, "Spam Cam 1", true, 1, "Numpad1", 5_000),
        new SpamKey(s, "Spam Cam 2", true, 1, "Numpad2", 5_000),
        new SpamKey(s, "Spam Cam 3", true, 1, "Numpad3", 5_000),
        new SpamKey(s, "Spam W", true, 1, "w", 2_000),
        new SpamKey(s, "Spam S", true, 1, "s", 2_000),
        new SpamKey(s, "Spam A", true, 1, "a", 2_000),
        new SpamKey(s, "Spam D", true, 1, "d", 2_000),

        new SpamTwoKeys(s, "Spam W S", true, 1, "w", "s", 2_000),
        new SpamTwoKeys(s, "Spam A D", true, 1, "a", "d", 2_000),

        new DisplayRotationEffect(s, "Rotate display to 90°", true, 1, DisplayRotationHelper.Orientations.DegreesCw90, 15_000),
        new DisplayRotationEffect(s, "Rotate display to 180°", true, 1, DisplayRotationHelper.Orientations.DegreesCw180, 15_000),
        new DisplayRotationEffect(s, "Rotate display to 270°", true, 1, DisplayRotationHelper.Orientations.DegreesCw270, 15_000),

        new AhkMutespam(s, "Spam system mute", true, 1, 5000),
        new PressKey(s, "Open Windows Magnifier", true, 1, "LWin down}{NumpadAdd}{LWin up"),

        new AhkMsgBox(s, "Show MessageBox", true, 1, "Hi :)"),
        new AhkTooltip(s, "Show many Tooltips", true, 1, "Hi :)", 400, 20),

        new ShowUnregisteredHypercam(s, "\"Unregistered Hypercam 2\" watermark", true, 1, 10_000),
        //@formatter:on
    };

    #endregion

    #region Properties

    private static readonly Random WeightRandom = new();

    private readonly EffectsFileWriter _effectsFileWriter;
    private readonly TextBoxLogger _textBoxLogger;

    public readonly ReadOnlyObservableCollection<EffectBase> EffectList;

    private EffectBase? _previousEffect = null;

    #endregion

    #region constructor

    public EffectsHandler(SettingsHandler settingsHandler, EffectsFileWriter effectsFileWriter,
        TextBoxLogger textBoxLogger)
    {
        _effectsFileWriter = effectsFileWriter;
        _textBoxLogger = textBoxLogger;
        EffectList = new ReadOnlyObservableCollection<EffectBase>(
            new ObservableCollection<EffectBase>(
                GetDefaultEffects(settingsHandler)
            )
        );
    }

    #endregion

    #region Methods

    private static long GetTotalWeight(IEnumerable<EffectBase> effects) =>
        effects.Where(effect => effect.Enabled).Sum(effect => effect.Weight);

    /// <summary>
    /// Weighted random selection<br/>
    /// https://www.educative.io/answers/what-is-the-weighted-random-selection-algorithm <br/>
    /// https://stackoverflow.com/questions/56692/random-weighted-choice
    /// </summary>
    /// <returns></returns>
    private EffectBase? SelectRandomEffectByWeight()
    {
        List<EffectBase> filteredEffects = EffectList
            .Where(effect => !effect.Active)
            // TODO: Don't allow the same effect twice in a row
            .Except(_previousEffect != null ? new[] { _previousEffect } : Array.Empty<EffectBase>())
            .Where(effect => effect.Enabled)
            .ToList();

        long randomIndexForWeightCalc = WeightRandom.NextInt64(0, GetTotalWeight(filteredEffects));

        foreach (EffectBase effect in filteredEffects)
        {
            if (randomIndexForWeightCalc < effect.Weight)
            {
                // TODO Temp disabled before I have a setting for it.
                //_previousEffect = effect;
                return effect;
            }

            randomIndexForWeightCalc -= effect.Weight;
        }

        // No effect found with that weight or no effects are defined in the first place.
        return null;
    }

    public void ExecuteRandom(string triggerUserName) => Task.Run(() => ExecuteTask(triggerUserName));

    private void ExecuteTask(string userNameWhoTriggeredTheEffect)
    {
        EffectBase? effect = SelectRandomEffectByWeight();

        if (effect != null)
        {
            _textBoxLogger.WriteLine(effect.GetConsoleOutput());
            _effectsFileWriter.AddNewEffect(effect, userNameWhoTriggeredTheEffect);
            effect.Execute();
            _effectsFileWriter.RemoveEffect(effect, userNameWhoTriggeredTheEffect);
        }
        else
            _textBoxLogger.WriteLine("📻 No TM Effects enabled or all effects already active.");
    }

    #endregion
}
