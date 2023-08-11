using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BitMusic.Helper;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.FileWriters;

public class EffectsFileWriter
{
    private readonly TextBoxLogger _textBoxLogger;
    private readonly FileInfo _outputFileInfo;

    private List<(EffectBase effect, string userNameWhoTriggeredTheEffect)> _activeEffects = new();

    public EffectsFileWriter(TextBoxLogger textBoxLogger, FileInfo outputFileInfo)
    {
        _textBoxLogger = textBoxLogger;
        _outputFileInfo = outputFileInfo;

        File.WriteAllText(_outputFileInfo.FullName, string.Empty);
    }

    public void AddNewEffect(EffectBase effect, string userNameWhoTriggeredTheEffect)
    {
        //File.WriteAllText(_outputFileInfo.FullName, line);

        _activeEffects.Add((effect, userNameWhoTriggeredTheEffect));
        UpdateText();
        Task.Run(() => EffectRunner(effect, userNameWhoTriggeredTheEffect));
    }


    private async void EffectRunner(EffectBase effectBase, string userNameWhoTriggeredTheEffect)
    {
        await Task.Delay(5000);
        _activeEffects.Remove((effectBase, userNameWhoTriggeredTheEffect));
        UpdateText();
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
