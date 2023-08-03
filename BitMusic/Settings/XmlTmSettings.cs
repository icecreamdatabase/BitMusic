using System.Xml.Serialization;

namespace BitMusic.Settings;

public class XmlTmSettings
{
    [XmlElement("ProcessName")]
    public string ProcessName = string.Empty;

    [XmlElement("BitAmount")]
    public int BitAmount;
}
