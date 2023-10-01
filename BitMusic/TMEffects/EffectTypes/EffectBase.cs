using System.Threading.Tasks;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BitMusic.TMEffects.EffectTypes;

public abstract class EffectBase : ObservableRecipient
{
    private IRelayCommand? _testButton;
    public IRelayCommand TestButton => _testButton ??= new RelayCommand(ExecuteWithDelay);

    private string _testButtonText = "Test";

    public string TestButtonText
    {
        get => _testButtonText;
        set => SetProperty(ref _testButtonText, value);
    }

    private readonly SettingsHandler _settingsHandler;
    private protected XmlTmSettings TmSettings => _settingsHandler.ActiveSettings.TmSettings;

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

    protected EffectBase(SettingsHandler settingsHandler, string displayName, bool enabled, uint weight)
    {
        _settingsHandler = settingsHandler;
        DisplayName = displayName;
        Enabled = enabled;
        Weight = weight;
    }

    public string GetConsoleOutput()
    {
        return $"✨ {DisplayName}";
    }

    public abstract void Execute();

    private void ExecuteWithDelay() => Task.Run(ExecuteWithDelayAsync);

    private async void ExecuteWithDelayAsync()
    {
        for (int i = 5; i >= 1; i--)
        {
            TestButtonText = i.ToString();
            await Task.Delay(1000);
        }

        TestButtonText = "Running";

        Execute();
        await Task.Delay(2000);
        TestButtonText = "Test";
    }
}
