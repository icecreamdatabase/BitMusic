using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BitMusic.ViewModel.PerTabViewModel;

public class TmEffectsViewModel : ObservableRecipient
{
    private string _settingsEffectBits = string.Empty;

    public string SettingsEffectBits
    {
        get => _settingsEffectBits;
        set => SetProperty(ref _settingsEffectBits, value);
    }

    private string _settingsExeName = string.Empty;

    public string SettingsExeName
    {
        get => _settingsExeName;
        set => SetProperty(ref _settingsExeName, value);
    }

    #region Properties and Fields

    private readonly BitMusicViewModel _bitMusicViewModel;

    #endregion

    #region Constructor and Overrides

    public TmEffectsViewModel(BitMusicViewModel bitMusicViewModel)
    {
        _bitMusicViewModel = bitMusicViewModel;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _bitMusicViewModel.SaveSettings();

        base.OnPropertyChanged(e);
    }

    #endregion

    #region Methods

    public void LoadSettings(SettingsHandler settingsHandler)
    {
        SettingsEffectBits = settingsHandler.ActiveSettings.TmSettings.BitAmount.ToString();
        SettingsExeName = settingsHandler.ActiveSettings.TmSettings.ProcessName;
    }

    public void SaveSettings(SettingsHandler settingsHandler)
    {
        settingsHandler.ActiveSettings.TmSettings.BitAmount = int.TryParse(SettingsEffectBits, out int parsedBits)
            ? parsedBits
            : 0;

        settingsHandler.ActiveSettings.TmSettings.ProcessName = SettingsExeName;
    }

    #endregion
}
