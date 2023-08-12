using BitMusic.Settings;

namespace BitMusic.TMEffects.EffectTypes;

public class DummyEffect : EffectBase
{
    public DummyEffect() : base("Dummy Effect", false, 0)
    {
    }

    public override void Execute(XmlTmSettings tmSettings)
    {
    }
}
