using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BitMusic.FileWriters;
using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.Settings;
using BitMusic.ViewModel.PerTabViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BitMusic.ViewModel;

[SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
public class BitMusicViewModel : ObservableRecipient
{
    #region Properties and Fields

    public MainTabViewModel MainTabViewModel { get; }
    public MusicSettingsViewModel MusicSettingsViewModel { get; }
    public TmEffectsViewModel TmEffectsViewModel { get; }

    private readonly BotInstance _botInstance;

    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly BitHandler _bitHandler;

    private readonly MusicPlayer _musicPlayer;
    private readonly ObsFileWriter _obsFileWriter;
    private readonly EffectsFileWriter _effectsFileWriter;

    //private readonly OBSWebsocket _ws;

    private bool _isLoadingData;

    #endregion

    #region Constructor and Overrides

    public BitMusicViewModel()
    {
        FileInfo settingsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.xml"));
        FileInfo obsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "obs.txt"));
        FileInfo obsEffectsFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "obsEffects.txt"));
        _settingsHandler = new SettingsHandler(settingsFile);

        _textBoxLogger = new TextBoxLogger(this);
        _obsFileWriter = new ObsFileWriter(_textBoxLogger, _settingsHandler, obsFile);
        _effectsFileWriter = new EffectsFileWriter(_textBoxLogger, obsEffectsFile);
        _musicPlayer = new MusicPlayer(this, _textBoxLogger);

        _botInstance = new BotInstance("justinfan1234", "1234");
        _bitHandler = new BitHandler(this, _textBoxLogger, _botInstance, _settingsHandler, _effectsFileWriter);

        _botInstance.OnNewIrcNamReply += NewIrcNamReply;

        MainTabViewModel = new(this, _textBoxLogger, _obsFileWriter, _musicPlayer, _botInstance);
        MusicSettingsViewModel = new(this);
        TmEffectsViewModel = new(this);

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

    #region Methods

    private void LoadSettings()
    {
        _isLoadingData = true;
        MainTabViewModel.LoadSettings(_settingsHandler);
        MusicSettingsViewModel.LoadSettings(_settingsHandler);
        TmEffectsViewModel.LoadSettings(_settingsHandler);
        _isLoadingData = false;
    }

    internal void SaveSettings()
    {
        if (_isLoadingData)
            return;

        MainTabViewModel.SaveSettings(_settingsHandler);
        MusicSettingsViewModel.SaveSettings(_settingsHandler);
        TmEffectsViewModel.SaveSettings(_settingsHandler);

        _settingsHandler.SaveSettingsToDisk();
        LoadSettings();
        //_musicPlayer.Reload();
    }

    private void NewIrcNamReply(string roomName, string userName)
    {
        _textBoxLogger.WriteLine($"Joined channel {roomName} as {userName}");
    }

    #endregion
}
