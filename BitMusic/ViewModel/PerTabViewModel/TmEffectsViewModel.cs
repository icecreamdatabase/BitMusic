using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BitMusic.Settings;
using BitMusic.TMEffects;
using BitMusic.TMEffects.EffectTypes;
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
        _effectList = new ObservableCollection<EffectBase>(EffectsHandler.DefaultEffects);
        foreach (EffectBase effectBase in _effectList)
        {
            effectBase.PropertyChanged += (_, args) => OnPropertyChanged(args);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _bitMusicViewModel.SaveSettings();

        base.OnPropertyChanged(e);
    }

    private ObservableCollection<EffectBase> _effectList; // TODO

    public ObservableCollection<EffectBase> EffectList
    {
        get => _effectList;
        private set => SetProperty(ref _effectList, value);
    }

    #endregion

    #region Methods

    public void LoadSettings(SettingsHandler settingsHandler)
    {
        SettingsEffectBits = settingsHandler.ActiveSettings.TmSettings.BitAmount.ToString();
        SettingsExeName = settingsHandler.ActiveSettings.TmSettings.ProcessName;

        foreach (XmlEffectSetting effectSetting in settingsHandler.ActiveSettings.TmSettings.EffectSettings)
        {
            EffectBase? effect = _effectList.FirstOrDefault(effect =>
                string.Equals(effect.DisplayName, effectSetting.DisplayName, StringComparison.OrdinalIgnoreCase)
            );

            if (effect == null)
                continue;

            effect.Weight = effectSetting.Weight;
            effect.Enabled = effectSetting.Enabled;
        }
    }

    public void SaveSettings(SettingsHandler settingsHandler)
    {
        settingsHandler.ActiveSettings.TmSettings.BitAmount = int.TryParse(SettingsEffectBits, out int parsedBits)
            ? parsedBits
            : 0;

        settingsHandler.ActiveSettings.TmSettings.ProcessName = SettingsExeName;

        settingsHandler.ActiveSettings.TmSettings.EffectSettings.Clear();
        foreach (EffectBase effectBase in _effectList)
        {
            settingsHandler.ActiveSettings.TmSettings.EffectSettings.Add(new XmlEffectSetting(effectBase));
        }
    }

    #endregion
}
