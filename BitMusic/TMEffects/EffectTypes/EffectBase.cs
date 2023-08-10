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

    private int _weight;

    public int Weight
    {
        get => _weight;
        set => SetProperty(ref _weight, value);
    }


    protected EffectBase(string displayName, bool enabled, int weight)
    {
        DisplayName = displayName;
        Enabled = enabled;
        Weight = weight;
    }

    public abstract string GetConsoleOutput();

    public abstract void Execute(string processName);
}
