using System;
using System.IO;
using BitMusic.Helper;
using BitMusic.Settings;

namespace BitMusic;

public class ObsFileWriter
{
    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly FileInfo _outputFileInfo;
    private string _previousText = Environment.NewLine;

    public ObsFileWriter(TextBoxLogger textBoxLogger, SettingsHandler settingsHandler, FileInfo outputFileInfo)
    {
        _textBoxLogger = textBoxLogger;
        _settingsHandler = settingsHandler;
        _outputFileInfo = outputFileInfo;

        File.WriteAllText(_outputFileInfo.FullName, _previousText);
    }

    public void UpdateText(double volumeSlider, double speedSlider)
    {
        string line = GetText(_settingsHandler.ActiveSettings.Volume, volumeSlider) +
                      Environment.NewLine +
                      GetText(_settingsHandler.ActiveSettings.Speed, speedSlider);

        if (line == _previousText)
            return;
        _previousText = line;

        File.WriteAllText(_outputFileInfo.FullName, line);
        _textBoxLogger.WriteLine("✏ Updated OBS text");
    }

    private static string GetText(XmlTypeSetting xmlTypeSetting, double sliderValue)
    {
        if (Math.Abs(sliderValue - xmlTypeSetting.Steps[0]) < 0.001d)
        {
            // reached min
            return xmlTypeSetting.MinText;
        }
        else if (Math.Abs(sliderValue - xmlTypeSetting.Steps[^1]) < 0.001d)
        {
            // reached max
            return xmlTypeSetting.MaxText;
        }
        else
        {
            // neither
            return string.Empty;
        }
    }
}
