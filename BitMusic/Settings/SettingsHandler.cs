using System.IO;

namespace BitMusic.Settings;

public class SettingsHandler
{
    public readonly XmlSettings ActiveSettings = new();
    private readonly FileInfo _settingsFile;

    public SettingsHandler(FileInfo settingsFile)
    {
        _settingsFile = settingsFile;

        if (_settingsFile.Exists)
        {
            ActiveSettings = XmlParser.XmlDeserializeFromString<XmlSettings>(File.ReadAllText(_settingsFile.FullName));
        }
    }

    public void SaveSettingsToDisk()
    {
        if (_settingsFile.Exists)
            _settingsFile.Delete();
        File.WriteAllText(_settingsFile.FullName, XmlParser.XmlSerializeToString(ActiveSettings));
    }
}
