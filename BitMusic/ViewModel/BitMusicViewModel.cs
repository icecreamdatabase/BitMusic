using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private IRelayCommand? _connectButton;
    public IRelayCommand ConnectButton => _connectButton ??= new RelayCommand(Connect);

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

    private Brush _connectButtonBackground = ConnectedButtonBackgroundNotConnected;

    private static readonly Brush ConnectedButtonBackgroundNotConnected = new SolidColorBrush();

    private static readonly Brush ConnectedButtonBackgroundConnected =
        new SolidColorBrush(Color.FromArgb(128, 0, 128, 0));

    private static readonly Brush ConnectedButtonBackgroundConnectionFailed =
        new SolidColorBrush(Color.FromArgb(128, 128, 0, 0));

    public Brush ConnectButtonBackground
    {
        get => _connectButtonBackground;
        private set => SetProperty(ref _connectButtonBackground, value);
    }

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
    //private readonly OBSWebsocket _ws;

    #endregion

    #region Constructor

    public BitMusicViewModel()
    {
        FileInfo settingsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
        _settingsHandler = new SettingsHandler(settingsFile);

        _textBoxLogger = new TextBoxLogger(this);
        _botInstance = new BotInstance("justinfan1234", "1234");
        _bitHandler = new BitHandler(_textBoxLogger, _settingsHandler, _botInstance);

        _musicPlayer = new MusicPlayer(this, _textBoxLogger);

        _botInstance.OnNewIrcNamReply += NewIrcNamReply;

        LoadSettings();

        //_ws = new OBSWebsocket();
        //_ws.ConnectAsync("ws://localhost:4455", "");
        //_ws.Connected += WsOnConnected;
    }

    //private void WsOnConnected(object? sender, EventArgs e)
    //{
    //    InputSettings? inputSettings = _ws.GetInputSettings("AYAYA");
    //    inputSettings.Settings["speed_percent"] = 100;
    //    _ws.SetInputSettings(inputSettings);
    //}

    #endregion

    #region Binding Methods

    private void Connect()
    {
        if (string.IsNullOrEmpty(ChannelTextBoxText))
            return;

        if (ConnectButtonBackground == ConnectedButtonBackgroundConnected)
        {
        }

        _botInstance.Channels.Set(ChannelTextBoxText);

        _textBoxLogger.WriteLine("Joining...");
        ConnectButtonBackground = ConnectedButtonBackgroundConnected;
    }

    private void LoadSettings()
    {
        ChannelTextBoxText = _settingsHandler.ActiveSettings.Channel;
        SettingsVolumeUp = _settingsHandler.ActiveSettings.VolumeUpBits.ToString();
        SettingsVolumeDown = _settingsHandler.ActiveSettings.VolumeDownBits.ToString();
        SettingsVolumeSteps = _settingsHandler.ActiveSettings.VolumeSteps;
        SettingsSpeedUp = _settingsHandler.ActiveSettings.SpeedUpBits.ToString();
        SettingsSpeedDown = _settingsHandler.ActiveSettings.SpeedDownBits.ToString();
        SettingsSpeedSteps = _settingsHandler.ActiveSettings.SpeedSteps;

        SongList = new ObservableCollection<SongItem>(
            _settingsHandler.ActiveSettings.AudioFiles.Select(path => new SongItem(path))
        );

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

        _settingsHandler.ActiveSettings.VolumeUpBits = int.TryParse(SettingsVolumeUp, out int settingsVolumeUp)
            ? settingsVolumeUp
            : 0;
        _settingsHandler.ActiveSettings.VolumeDownBits = int.TryParse(SettingsVolumeDown, out int settingsVolumeDown)
            ? settingsVolumeDown
            : 0;
        _settingsHandler.ActiveSettings.VolumeSteps = SettingsVolumeSteps;

        _settingsHandler.ActiveSettings.SpeedUpBits = int.TryParse(SettingsSpeedUp, out int settingsSpeedUp)
            ? settingsSpeedUp
            : 0;
        _settingsHandler.ActiveSettings.SpeedDownBits = int.TryParse(SettingsSpeedDown, out int settingsSpeedDown)
            ? settingsSpeedDown
            : 0;
        _settingsHandler.ActiveSettings.SpeedSteps = SettingsSpeedSteps;

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

    #endregion

    #region Methods

    private void NewIrcNamReply(string roomName, string userName)
    {
        _textBoxLogger.WriteLine($"Joined channel {roomName} as {userName}");
    }

    #endregion
}
