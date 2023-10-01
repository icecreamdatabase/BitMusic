using System.Threading.Tasks;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BitMusic.TMEffects.EffectTypes;

public abstract class EffectBase : ObservableRecipient
{
    private IRelayCommand? _testButton;
    public IRelayCommand TestButton => _testButton ??= new RelayCommand(TestWithDelay);

    private string _testButtonText = "Test";

    public string TestButtonText
    {
        get => _testButtonText;
        private set => SetProperty(ref _testButtonText, value);
    }

    private readonly SettingsHandler _settingsHandler;
    private protected XmlTmSettings TmSettings => _settingsHandler.ActiveSettings.TmSettings;

    public string DisplayName { get; }

    public bool Active { get; private set; }

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

    public void Execute()
    {
        Active = true;
        try
        {
            ExecuteRaw();
        }
        finally
        {
            Active = false;
        }
    }

    private protected abstract void ExecuteRaw();

    private void TestWithDelay() => Task.Run(TestWithDelayAsync);

    private async void TestWithDelayAsync()
    {
        for (int i = 5; i >= 1; i--)
        {
            TestButtonText = i.ToString();
            await Task.Delay(1000);
        }

        TestButtonText = "Running";

        ExecuteRaw();
        TestButtonText = "Test";
    }
}
