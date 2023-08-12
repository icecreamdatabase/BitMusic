using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BitMusic.TMEffects.EffectTypes;

public abstract class EffectBase : ObservableRecipient
{
    public string DisplayName { get; }

    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set => SetProperty(ref _enabled, value);
    }

    private uint _weight;

    public uint Weight
    {
        get => _weight;
        set => SetProperty(ref _weight, value);
    }

    protected EffectBase(string displayName, bool enabled, uint weight)
    {
        DisplayName = displayName;
        Enabled = enabled;
        Weight = weight;
    }

    public string GetConsoleOutput()
    {
        return $"✨ {DisplayName}";
    }

    public abstract void Execute(XmlTmSettings tmSettings);
}
