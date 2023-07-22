using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace BitMusic.ViewModel;

public class BitMusicViewModel : ObservableRecipient
{
    #region RelayCommandsForButtons

    private IRelayCommand? _playPauseButton;
    public IRelayCommand PlayPauseButton => _playPauseButton ??= new RelayCommand(_musicPlayer.PlayPause);

    private IRelayCommand? _skipButton;
    public IRelayCommand SkipButton => _skipButton ??= new RelayCommand(_musicPlayer.NextSong);

    private IRelayCommand? _settingsSaveButton;
    public IRelayCommand SettingsSaveButton => _settingsSaveButton ??= new RelayCommand(SaveSettings);

    private IRelayCommand? _songAddNewButton;

    public IRelayCommand SongAddNewButton => _songAddNewButton ??= new RelayCommand(SongAddNew);

    private IRelayCommand? _songDeleteKey;

    public IRelayCommand SongItemDeleteKey => _songDeleteKey ??= new RelayCommand(SongDelete);

    #endregion

    #region Bound Properties

    private bool _shuffleCheckbox = true;

    public bool ShuffleCheckbox
    {
        get => _shuffleCheckbox;
        set => SetProperty(ref _shuffleCheckbox, value);
    }

    private bool _repeatCheckbox = false;

    public bool RepeatCheckbox
    {
        get => _repeatCheckbox;
        set => SetProperty(ref _repeatCheckbox, value);
    }

    private double _volumeSlider = 50;

    public double VolumeSlider
    {
        get => _volumeSlider;
        set
        {
            SetProperty(ref _volumeSlider, value);
            _musicPlayer.Volume = value;
        }
    }

    private double _speedSplider = 100;

    public double SpeedSlider
    {
        get => _speedSplider;
        set
        {
            SetProperty(ref _speedSplider, value);
            _musicPlayer.SpeedRatio = value;
        }
    }

    private string _channelTextBoxText = string.Empty;

    public string ChannelTextBoxText
    {
        get => _channelTextBoxText;
        set => SetProperty(ref _channelTextBoxText, value);
    }

    private string _textBoxText = string.Empty;

    public string TextBoxText
    {
        get => _textBoxText;
        set => SetProperty(ref _textBoxText, value);
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

    private readonly BotInstance _botInstance;

    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly BitHandler _bitHandler;

    private readonly MusicPlayer _musicPlayer;
    private readonly ObsFileWriter _obsFileWriter;

    //private readonly OBSWebsocket _ws;

    #endregion

    #region Constructor and Overrides

    public BitMusicViewModel()
    {
        FileInfo settingsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
        FileInfo obsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "obs.txt"));
        _settingsHandler = new SettingsHandler(settingsFile);

        _textBoxLogger = new TextBoxLogger(this);
        _musicPlayer = new MusicPlayer(this, _textBoxLogger);

        _botInstance = new BotInstance("justinfan1234", "1234");
        _bitHandler = new BitHandler(this, _textBoxLogger, _botInstance, _settingsHandler);
        _obsFileWriter = new ObsFileWriter(_textBoxLogger, _settingsHandler, obsFile);

        _botInstance.OnNewIrcNamReply += NewIrcNamReply;

        LoadSettings();

        //_ws = new OBSWebsocket();
        //_ws.ConnectAsync("ws://localhost:4455", "");
        //_ws.Connected += WsOnConnected;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(VolumeSlider) or nameof(SpeedSlider))
            _obsFileWriter.UpdateText(VolumeSlider, SpeedSlider);

        base.OnPropertyChanged(e);
    }

    //private void WsOnConnected(object? sender, EventArgs e)
    //{
    //    InputSettings? inputSettings = _ws.GetInputSettings("AYAYA");
    //    inputSettings.Settings["speed_percent"] = 100;
    //    _ws.SetInputSettings(inputSettings);
    //}

    #endregion

    #region Methods

    private void LoadSettings()
    {
        ChannelTextBoxText = _settingsHandler.ActiveSettings.Channel;
        SettingsVolumeUp = _settingsHandler.ActiveSettings.Volume.Up.ToString();
        SettingsVolumeDown = _settingsHandler.ActiveSettings.Volume.Down.ToString();
        SettingsVolumeMax = _settingsHandler.ActiveSettings.Volume.Max.ToString();
        SettingsVolumeMin = _settingsHandler.ActiveSettings.Volume.Min.ToString();
        SettingsVolumeMaxText = _settingsHandler.ActiveSettings.Volume.MaxText;
        SettingsVolumeMinText = _settingsHandler.ActiveSettings.Volume.MinText;
        SettingsVolumeSteps = _settingsHandler.ActiveSettings.Volume.StepsString;
        SettingsSpeedUp = _settingsHandler.ActiveSettings.Speed.Up.ToString();
        SettingsSpeedDown = _settingsHandler.ActiveSettings.Speed.Down.ToString();
        SettingsSpeedMax = _settingsHandler.ActiveSettings.Speed.Max.ToString();
        SettingsSpeedMin = _settingsHandler.ActiveSettings.Speed.Min.ToString();
        SettingsSpeedMaxText = _settingsHandler.ActiveSettings.Speed.MaxText;
        SettingsSpeedMinText = _settingsHandler.ActiveSettings.Speed.MinText;
        SettingsSpeedSteps = _settingsHandler.ActiveSettings.Speed.StepsString;

        SongList = new ObservableCollection<SongItem>(
            _settingsHandler.ActiveSettings.AudioFiles.Select(path => new SongItem(path))
        );

        _obsFileWriter.UpdateText(VolumeSlider, SpeedSlider);

        if (!_botInstance.Channels.Contains(ChannelTextBoxText))
        {
            if (_botInstance.Channels.Count > 0)
                _textBoxLogger.WriteLine($"Leaving channel: {_botInstance.Channels.FirstOrDefault()}");

            _textBoxLogger.WriteLine($"Joining channel: {ChannelTextBoxText}");
        }

        _botInstance.Channels.Set(ChannelTextBoxText);
    }

    private void SaveSettings()
    {
        _settingsHandler.ActiveSettings.Channel = ChannelTextBoxText;

        _settingsHandler.ActiveSettings.Volume.Up = int.TryParse(SettingsVolumeUp, out int settingsVolumeUp)
            ? settingsVolumeUp
            : 0;
        _settingsHandler.ActiveSettings.Volume.Down = int.TryParse(SettingsVolumeDown, out int settingsVolumeDown)
            ? settingsVolumeDown
            : 0;
        _settingsHandler.ActiveSettings.Volume.Max = int.TryParse(SettingsVolumeMax, out int settingsVolumeMax)
            ? settingsVolumeMax
            : 0;
        _settingsHandler.ActiveSettings.Volume.Min = int.TryParse(SettingsVolumeMin, out int settingsVolumeMin)
            ? settingsVolumeMin
            : 0;
        _settingsHandler.ActiveSettings.Volume.MaxText = SettingsVolumeMaxText;
        _settingsHandler.ActiveSettings.Volume.MinText = SettingsVolumeMinText;
        _settingsHandler.ActiveSettings.Volume.StepsString = SettingsVolumeSteps;

        _settingsHandler.ActiveSettings.Speed.Up = int.TryParse(SettingsSpeedUp, out int settingsSpeedUp)
            ? settingsSpeedUp
            : 0;
        _settingsHandler.ActiveSettings.Speed.Down = int.TryParse(SettingsSpeedDown, out int settingsSpeedDown)
            ? settingsSpeedDown
            : 0;
        _settingsHandler.ActiveSettings.Speed.Max = int.TryParse(SettingsSpeedMax, out int settingsSpeedMax)
            ? settingsSpeedMax
            : 0;
        _settingsHandler.ActiveSettings.Speed.Min = int.TryParse(SettingsSpeedMin, out int settingsSpeedMin)
            ? settingsSpeedMin
            : 0;
        _settingsHandler.ActiveSettings.Speed.MaxText = SettingsSpeedMaxText;
        _settingsHandler.ActiveSettings.Speed.MinText = SettingsSpeedMinText;
        _settingsHandler.ActiveSettings.Speed.StepsString = SettingsSpeedSteps;

        _settingsHandler.ActiveSettings.AudioFiles = SongList.Select(songItem => songItem.FileInfo.FullName).ToList();

        _settingsHandler.SaveSettingsToDisk();
        LoadSettings();
        //_musicPlayer.Reload();
    }

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
        }
    }

    private void SongDelete()
    {
        List<SongItem> songItemsToDelete = SongList.Where(songItem => songItem.IsSelected).ToList();
        foreach (SongItem songItem in songItemsToDelete)
            SongList.Remove(songItem);
    }

    private void NewIrcNamReply(string roomName, string userName)
    {
        _textBoxLogger.WriteLine($"Joined channel {roomName} as {userName}");
    }

    #endregion
}
