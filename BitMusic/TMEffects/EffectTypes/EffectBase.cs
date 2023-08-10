namespace BitMusic.TMEffects.EffectTypes;

public abstract class EffectBase
{
    public string DisplayName { get; }
    public bool Enabled { get; set; }
    public int Weight { get; set; }
    

    protected EffectBase(string displayName, bool enabled, int weight)
    {
        DisplayName = displayName;
        Enabled = enabled;
        Weight = weight;
    }

    public abstract string GetConsoleOutput();

    public abstract void Execute(string processName);
}
