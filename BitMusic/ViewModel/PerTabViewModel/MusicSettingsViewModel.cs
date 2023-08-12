using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace BitMusic.ViewModel.PerTabViewModel;

public class MusicSettingsViewModel : ObservableRecipient
{
    #region RelayCommandsForButtons

    private IRelayCommand? _songAddNewButton;

    public IRelayCommand SongAddNewButton => _songAddNewButton ??= new RelayCommand(SongAddNew);

    private IRelayCommand? _songDeleteKey;

    public IRelayCommand SongItemDeleteKey => _songDeleteKey ??= new RelayCommand(SongDelete);

    #endregion

    #region Bound Properties

    private uint _settingsSkip;

    public uint SettingsSkip
    {
        get => _settingsSkip;
        set => SetProperty(ref _settingsSkip, value);
    }

    private uint _settingsVolumeUp;

    public uint SettingsVolumeUp
    {
        get => _settingsVolumeUp;
        set => SetProperty(ref _settingsVolumeUp, value);
    }

    private uint _settingsVolumeDown;

    public uint SettingsVolumeDown
    {
        get => _settingsVolumeDown;
        set => SetProperty(ref _settingsVolumeDown, value);
    }

    private uint _settingsVolumeMax;

    public uint SettingsVolumeMax
    {
        get => _settingsVolumeMax;
        set => SetProperty(ref _settingsVolumeMax, value);
    }

    private uint _settingsVolumeMin;

    public uint SettingsVolumeMin
    {
        get => _settingsVolumeMin;
        set => SetProperty(ref _settingsVolumeMin, value);
    }

    private string _settingsVolumeMaxText = string.Empty;

    public string SettingsVolumeMaxText
    {
        get => _settingsVolumeMaxText;
        set => SetProperty(ref _settingsVolumeMaxText, value);
    }

    private string _settingsVolumeMinText = string.Empty;

    public string SettingsVolumeMinText
    {
        get => _settingsVolumeMinText;
        set => SetProperty(ref _settingsVolumeMinText, value);
    }

    private string _settingsVolumeSteps = string.Empty;

    public string SettingsVolumeSteps
    {
        get => _settingsVolumeSteps;
        set => SetProperty(ref _settingsVolumeSteps, value);
    }

    private uint _settingsSpeedUp;

    public uint SettingsSpeedUp
    {
        get => _settingsSpeedUp;
        set => SetProperty(ref _settingsSpeedUp, value);
    }

    private uint _settingsSpeedDown;

    public uint SettingsSpeedDown
    {
        get => _settingsSpeedDown;
        set => SetProperty(ref _settingsSpeedDown, value);
    }

    private uint _settingsSpeedMax;

    public uint SettingsSpeedMax
    {
        get => _settingsSpeedMax;
        set => SetProperty(ref _settingsSpeedMax, value);
    }

    private uint _settingsSpeedMin;

    public uint SettingsSpeedMin
    {
        get => _settingsSpeedMin;
        set => SetProperty(ref _settingsSpeedMin, value);
    }

    private string _settingsSpeedMaxText = string.Empty;

    public string SettingsSpeedMaxText
    {
        get => _settingsSpeedMaxText;
        set => SetProperty(ref _settingsSpeedMaxText, value);
    }

    private string _settingsSpeedMinText = string.Empty;

    public string SettingsSpeedMinText
    {
        get => _settingsSpeedMinText;
        set => SetProperty(ref _settingsSpeedMinText, value);
    }

    private string _settingsSpeedSteps = string.Empty;

    public string SettingsSpeedSteps
    {
        get => _settingsSpeedSteps;
        set => SetProperty(ref _settingsSpeedSteps, value);
    }

    private ObservableCollection<SongItem> _songList = new();

    public ObservableCollection<SongItem> SongList
    {
        get => _songList;
        private set => SetProperty(ref _songList, value);
    }

    #endregion

    #region Properties and Fields

    private readonly BitMusicViewModel _bitMusicViewModel;

    #endregion

    #region Constructor and Overrides

    public MusicSettingsViewModel(BitMusicViewModel bitMusicViewModel)
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

    private void SongAddNew()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "Audio files (*.acc,*.m4a,*.mp3,*.wav,*.wma)|*.acc;*.m4a;*.mp3;*.wav;*.wma|All files (*.*)|*.*",
            Multiselect = true
        };
        if (openFileDialog.ShowDialog() == true)
        {
            foreach (SongItem songItem in openFileDialog.FileNames.Select(path => new SongItem(path)))
            {
                //if (!SongList.Contains(songItem))
                SongList.Add(songItem);
            }

            _bitMusicViewModel.SaveSettings();
        }
    }

    private void SongDelete()
    {
        List<SongItem> songItemsToDelete = SongList.Where(songItem => songItem.IsSelected).ToList();
        foreach (SongItem songItem in songItemsToDelete)
            SongList.Remove(songItem);
        _bitMusicViewModel.SaveSettings();
    }

    public void LoadSettings(SettingsHandler settingsHandler)
    {
        SettingsSkip = settingsHandler.ActiveSettings.Skip;
        
        SettingsVolumeUp = settingsHandler.ActiveSettings.Volume.Up;
        SettingsVolumeDown = settingsHandler.ActiveSettings.Volume.Down;
        SettingsVolumeMax = settingsHandler.ActiveSettings.Volume.Max;
        SettingsVolumeMin = settingsHandler.ActiveSettings.Volume.Min;
        SettingsVolumeMaxText = settingsHandler.ActiveSettings.Volume.MaxText;
        SettingsVolumeMinText = settingsHandler.ActiveSettings.Volume.MinText;
        SettingsVolumeSteps = settingsHandler.ActiveSettings.Volume.StepsString;
        
        SettingsSpeedUp = settingsHandler.ActiveSettings.Speed.Up;
        SettingsSpeedDown = settingsHandler.ActiveSettings.Speed.Down;
        SettingsSpeedMax = settingsHandler.ActiveSettings.Speed.Max;
        SettingsSpeedMin = settingsHandler.ActiveSettings.Speed.Min;
        SettingsSpeedMaxText = settingsHandler.ActiveSettings.Speed.MaxText;
        SettingsSpeedMinText = settingsHandler.ActiveSettings.Speed.MinText;
        SettingsSpeedSteps = settingsHandler.ActiveSettings.Speed.StepsString;

        SongList = new ObservableCollection<SongItem>(
            settingsHandler.ActiveSettings.AudioFiles.Select(path => new SongItem(path))
        );
    }

    public void SaveSettings(SettingsHandler settingsHandler)
    {
        settingsHandler.ActiveSettings.Skip = SettingsSkip;

        settingsHandler.ActiveSettings.Volume.Up = SettingsVolumeUp;
        settingsHandler.ActiveSettings.Volume.Down = SettingsVolumeDown;
        settingsHandler.ActiveSettings.Volume.Max = SettingsVolumeMax;
        settingsHandler.ActiveSettings.Volume.Min = SettingsVolumeMin;
        settingsHandler.ActiveSettings.Volume.MaxText = SettingsVolumeMaxText;
        settingsHandler.ActiveSettings.Volume.MinText = SettingsVolumeMinText;
        settingsHandler.ActiveSettings.Volume.StepsString = SettingsVolumeSteps;

        settingsHandler.ActiveSettings.Speed.Up = SettingsSpeedUp;
        settingsHandler.ActiveSettings.Speed.Down = SettingsSpeedDown;
        settingsHandler.ActiveSettings.Speed.Max = SettingsSpeedMax;
        settingsHandler.ActiveSettings.Speed.Min = SettingsSpeedMin;
        settingsHandler.ActiveSettings.Speed.MaxText = SettingsSpeedMaxText;
        settingsHandler.ActiveSettings.Speed.MinText = SettingsSpeedMinText;
        settingsHandler.ActiveSettings.Speed.StepsString = SettingsSpeedSteps;

        settingsHandler.ActiveSettings.AudioFiles = SongList.Select(songItem => songItem.FileInfo.FullName).ToList();
    }

    #endregion
}
