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
    #region Properties

    private uint _settingsEffectBits = 0;

    public uint SettingsEffectBits
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
    
    private uint _settingsMainDisplayNumber = 1;

    public uint SettingsMainDisplayNumber
    {
        get => _settingsMainDisplayNumber;
        set => SetProperty(ref _settingsMainDisplayNumber, value);
    }

    private ObservableCollection<EffectBase> _effectList;

    public ObservableCollection<EffectBase> EffectList
    {
        get => _effectList;
        private set => SetProperty(ref _effectList, value);
    }

    #endregion

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

    #endregion

    #region Methods

    public void LoadSettings(SettingsHandler settingsHandler)
    {
        SettingsEffectBits = settingsHandler.ActiveSettings.TmSettings.BitAmount;
        SettingsExeName = settingsHandler.ActiveSettings.TmSettings.ProcessName;
        SettingsMainDisplayNumber = settingsHandler.ActiveSettings.TmSettings.MainDisplayNumber;

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
        settingsHandler.ActiveSettings.TmSettings.BitAmount = SettingsEffectBits;

        settingsHandler.ActiveSettings.TmSettings.ProcessName = SettingsExeName;
        settingsHandler.ActiveSettings.TmSettings.MainDisplayNumber = SettingsMainDisplayNumber;

        settingsHandler.ActiveSettings.TmSettings.EffectSettings.Clear();
        foreach (EffectBase effectBase in _effectList)
        {
            settingsHandler.ActiveSettings.TmSettings.EffectSettings.Add(new XmlEffectSetting(effectBase));
        }
    }

    #endregion
}
