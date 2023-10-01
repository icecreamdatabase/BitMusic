using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitMusic.Helper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.FileWriters;

public class EffectsFileWriter
{
    private readonly TextBoxLogger _textBoxLogger;
    private readonly FileInfo _outputFileInfo;

    private readonly List<(EffectBase effect, string userNameWhoTriggeredTheEffect)> _activeEffects = new();
    private readonly object _activeEffectsLock = new();

    public EffectsFileWriter(TextBoxLogger textBoxLogger, FileInfo outputFileInfo)
    {
        _textBoxLogger = textBoxLogger;
        _outputFileInfo = outputFileInfo;

        File.WriteAllText(_outputFileInfo.FullName, string.Empty);
    }

    public void AddNewEffect(EffectBase effect, string userNameWhoTriggeredTheEffect)
    {
        lock (_activeEffectsLock)
        {
            _activeEffects.Add((effect, userNameWhoTriggeredTheEffect));
            UpdateText();
        }
    }

    public void RemoveEffect(EffectBase effect, string userNameWhoTriggeredTheEffect)
    {
        lock (_activeEffectsLock)
        {
            _activeEffects.Remove((effect, userNameWhoTriggeredTheEffect));
            UpdateText();
        }
    }

    private void UpdateText()
    {
        string fileContent = string.Join(
            Environment.NewLine,
            _activeEffects.Select(activeEffect =>
                $"\"{activeEffect.effect.DisplayName}\" by {activeEffect.userNameWhoTriggeredTheEffect}"
            )
        );
        File.WriteAllText(_outputFileInfo.FullName, fileContent);

        _textBoxLogger.WriteLine("✏ Updated OBS Effect text");
    }
}
