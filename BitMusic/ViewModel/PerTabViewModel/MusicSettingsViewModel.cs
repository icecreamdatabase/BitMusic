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

    private string _settingsSkip = string.Empty;

    public string SettingsSkip
    {
        get => _settingsSkip;
        set => SetProperty(ref _settingsSkip, value);
    }

    private string _settingsVolumeUp = string.Empty;

    public string SettingsVolumeUp
    {
        get => _settingsVolumeUp;
        set => SetProperty(ref _settingsVolumeUp, value);
    }

    private string _settingsVolumeDown = string.Empty;

    public string SettingsVolumeDown
    {
        get => _settingsVolumeDown;
        set => SetProperty(ref _settingsVolumeDown, value);
    }

    private string _settingsVolumeMax = string.Empty;

    public string SettingsVolumeMax
    {
        get => _settingsVolumeMax;
        set => SetProperty(ref _settingsVolumeMax, value);
    }

    private string _settingsVolumeMin = string.Empty;

    public string SettingsVolumeMin
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

    private string _settingsSpeedUp = string.Empty;

    public string SettingsSpeedUp
    {
        get => _settingsSpeedUp;
        set => SetProperty(ref _settingsSpeedUp, value);
    }

    private string _settingsSpeedDown = string.Empty;

    public string SettingsSpeedDown
    {
        get => _settingsSpeedDown;
        set => SetProperty(ref _settingsSpeedDown, value);
    }

    private string _settingsSpeedMax = string.Empty;

    public string SettingsSpeedMax
    {
        get => _settingsSpeedMax;
        set => SetProperty(ref _settingsSpeedMax, value);
    }

    private string _settingsSpeedMin = string.Empty;

    public string SettingsSpeedMin
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
        SettingsSkip = settingsHandler.ActiveSettings.Skip.ToString();
        SettingsVolumeUp = settingsHandler.ActiveSettings.Volume.Up.ToString();
        SettingsVolumeDown = settingsHandler.ActiveSettings.Volume.Down.ToString();
        SettingsVolumeMax = settingsHandler.ActiveSettings.Volume.Max.ToString();
        SettingsVolumeMin = settingsHandler.ActiveSettings.Volume.Min.ToString();
        SettingsVolumeMaxText = settingsHandler.ActiveSettings.Volume.MaxText;
        SettingsVolumeMinText = settingsHandler.ActiveSettings.Volume.MinText;
        SettingsVolumeSteps = settingsHandler.ActiveSettings.Volume.StepsString;
        SettingsSpeedUp = settingsHandler.ActiveSettings.Speed.Up.ToString();
        SettingsSpeedDown = settingsHandler.ActiveSettings.Speed.Down.ToString();
        SettingsSpeedMax = settingsHandler.ActiveSettings.Speed.Max.ToString();
        SettingsSpeedMin = settingsHandler.ActiveSettings.Speed.Min.ToString();
        SettingsSpeedMaxText = settingsHandler.ActiveSettings.Speed.MaxText;
        SettingsSpeedMinText = settingsHandler.ActiveSettings.Speed.MinText;
        SettingsSpeedSteps = settingsHandler.ActiveSettings.Speed.StepsString;

        SongList = new ObservableCollection<SongItem>(
            settingsHandler.ActiveSettings.AudioFiles.Select(path => new SongItem(path))
        );
    }

    public void SaveSettings(SettingsHandler settingsHandler)
    {
        settingsHandler.ActiveSettings.Skip = int.TryParse(SettingsSkip, out int settingsSkip)
            ? settingsSkip
            : 0;

        settingsHandler.ActiveSettings.Volume.Up = int.TryParse(SettingsVolumeUp, out int settingsVolumeUp)
            ? settingsVolumeUp
            : 0;
        settingsHandler.ActiveSettings.Volume.Down = int.TryParse(SettingsVolumeDown, out int settingsVolumeDown)
            ? settingsVolumeDown
            : 0;
        settingsHandler.ActiveSettings.Volume.Max = int.TryParse(SettingsVolumeMax, out int settingsVolumeMax)
            ? settingsVolumeMax
            : 0;
        settingsHandler.ActiveSettings.Volume.Min = int.TryParse(SettingsVolumeMin, out int settingsVolumeMin)
            ? settingsVolumeMin
            : 0;
        settingsHandler.ActiveSettings.Volume.MaxText = SettingsVolumeMaxText;
        settingsHandler.ActiveSettings.Volume.MinText = SettingsVolumeMinText;
        settingsHandler.ActiveSettings.Volume.StepsString = SettingsVolumeSteps;

        settingsHandler.ActiveSettings.Speed.Up = int.TryParse(SettingsSpeedUp, out int settingsSpeedUp)
            ? settingsSpeedUp
            : 0;
        settingsHandler.ActiveSettings.Speed.Down = int.TryParse(SettingsSpeedDown, out int settingsSpeedDown)
            ? settingsSpeedDown
            : 0;
        settingsHandler.ActiveSettings.Speed.Max = int.TryParse(SettingsSpeedMax, out int settingsSpeedMax)
            ? settingsSpeedMax
            : 0;
        settingsHandler.ActiveSettings.Speed.Min = int.TryParse(SettingsSpeedMin, out int settingsSpeedMin)
            ? settingsSpeedMin
            : 0;
        settingsHandler.ActiveSettings.Speed.MaxText = SettingsSpeedMaxText;
        settingsHandler.ActiveSettings.Speed.MinText = SettingsSpeedMinText;
        settingsHandler.ActiveSettings.Speed.StepsString = SettingsSpeedSteps;

        settingsHandler.ActiveSettings.AudioFiles = SongList.Select(songItem => songItem.FileInfo.FullName).ToList();
    }

    #endregion
}
