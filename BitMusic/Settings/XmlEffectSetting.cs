using System.Xml.Serialization;
using BitMusic.TMEffects.EffectTypes;

namespace BitMusic.Settings;

public class XmlEffectSetting
{
    [XmlAttribute]
    public string DisplayName = string.Empty;

    [XmlAttribute]
    public bool Enabled;

    [XmlAttribute]
    public int Weight = 1;

    public XmlEffectSetting()
    {
    }

    public XmlEffectSetting(EffectBase effectBase)
    {
        DisplayName = effectBase.DisplayName;
        Enabled = effectBase.Enabled;
        Weight = effectBase.Weight;
    }
}
