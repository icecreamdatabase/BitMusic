using System.Collections.Generic;
using System.Xml.Serialization;

namespace BitMusic.Settings;

public class XmlTmSettings
{
    [XmlElement("ProcessName")]
    public string ProcessName = string.Empty;

    [XmlElement("BitAmount")]
    public int BitAmount;
    
    [XmlArray("Effects")]
    [XmlArrayItem("Effect")]
    public List<XmlEffectSetting> EffectSettings = new();
}
