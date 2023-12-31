using System.Collections.Generic;
using System.Xml.Serialization;

namespace BitMusic.Settings;

[XmlRoot]
public class XmlSettings
{
    [XmlElement("Channel")]
    public string Channel = string.Empty;

    [XmlElement("MusicEnabled")]
    public bool MusicEnabled;

    [XmlElement("EffectsEnabled")]
    public bool EffectsEnabled;

    [XmlElement("TmSettings")]
    public XmlTmSettings TmSettings = new()
    {
        ProcessName = "Trackmania.exe",
        BitAmount = 123,
        MainDisplayNumber = 1
    };

    [XmlElement("Skip")]
    public uint Skip;

    [XmlElement("Volume")]
    public XmlTypeSetting Volume = new()
    {
        Steps = new double[] { 0, 2, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }
    };

    [XmlElement("Speed")]
    public XmlTypeSetting Speed = new()
    {
        Steps = new double[] { 0, 20, 40, 60, 80, 100, 120, 140, 160, 180, 200 }
    };

    [XmlArray("AudioFiles")]
    [XmlArrayItem("AudioFile")]
    public List<string> AudioFiles = new();
}
