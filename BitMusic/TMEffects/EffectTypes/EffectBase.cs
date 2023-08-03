namespace BitMusic.TMEffects.EffectTypes;

public abstract class EffectBase
{
    public int Weight { get; }

    protected EffectBase(int weight)
    {
        Weight = weight;
    }

    public abstract string GetConsoleOutput();

    public abstract void Execute(string processName);
}
