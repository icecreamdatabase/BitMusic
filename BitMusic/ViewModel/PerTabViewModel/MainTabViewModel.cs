using System.ComponentModel;
using System.Linq;
using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BitMusic.ViewModel.PerTabViewModel;

public class MainTabViewModel : ObservableRecipient
{
    #region RelayCommandsForButtons

    private IRelayCommand? _playPauseButton;
    public IRelayCommand PlayPauseButton => _playPauseButton ??= new RelayCommand(_musicPlayer.PlayPause);

    private IRelayCommand? _skipButton;
    public IRelayCommand SkipButton => _skipButton ??= new RelayCommand(SkipSong);

    //private IRelayCommand? _settingsSaveButton;
    //public IRelayCommand SettingsSaveButton => _settingsSaveButton ??= new RelayCommand(SaveSettings);

    private string _channelTextBoxText = string.Empty;

    public string ChannelTextBoxText
    {
        get => _channelTextBoxText;
        set => SetProperty(ref _channelTextBoxText, value);
    }

    #endregion

    #region Bound Properties

    private bool _musicEnabledCheckbox = true;

    public bool MusicEnabledCheckbox
    {
        get => _musicEnabledCheckbox;
        set => SetProperty(ref _musicEnabledCheckbox, value);
    }

    private bool _effectsEnabledCheckbox = false;

    public bool EffectsEnabledCheckbox
    {
        get => _effectsEnabledCheckbox;
        set => SetProperty(ref _effectsEnabledCheckbox, value);
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

    private string _textBoxText = string.Empty;

    public string TextBoxText
    {
        get => _textBoxText;
        set => SetProperty(ref _textBoxText, value);
    }

    #endregion

    #region Properties and Fields

    private readonly BitMusicViewModel _bitMusicViewModel;
    private readonly TextBoxLogger _textBoxLogger;
    private readonly ObsFileWriter _obsFileWriter;
    private readonly MusicPlayer _musicPlayer;
    private readonly BotInstance _botInstance;

    #endregion

    #region Constructor and Overrides

    public MainTabViewModel(BitMusicViewModel bitMusicViewModel,
        TextBoxLogger textBoxLogger, ObsFileWriter obsFileWriter, MusicPlayer musicPlayer, BotInstance botInstance)
    {
        _bitMusicViewModel = bitMusicViewModel;
        _textBoxLogger = textBoxLogger;
        _obsFileWriter = obsFileWriter;
        _musicPlayer = musicPlayer;
        _botInstance = botInstance;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(VolumeSlider) or nameof(SpeedSlider))
            _obsFileWriter.UpdateText(VolumeSlider, SpeedSlider);
        else
            _bitMusicViewModel.SaveSettings();

        base.OnPropertyChanged(e);
    }

    #endregion

    #region Methods

    public void SkipSong() => _musicPlayer.NextSong();

    public void LoadSettings(SettingsHandler settingsHandler)
    {
        MusicEnabledCheckbox = settingsHandler.ActiveSettings.MusicEnabled;
        EffectsEnabledCheckbox = settingsHandler.ActiveSettings.EffectsEnabled;
        ChannelTextBoxText = settingsHandler.ActiveSettings.Channel;

        if (!_botInstance.Channels.Contains(ChannelTextBoxText))
        {
            if (_botInstance.Channels.Count > 0)
                _textBoxLogger.WriteLine($"Leaving channel: {_botInstance.Channels.FirstOrDefault()}");

            _textBoxLogger.WriteLine($"Joining channel: {ChannelTextBoxText}");
        }

        _botInstance.Channels.Set(ChannelTextBoxText);

        _obsFileWriter.UpdateText(VolumeSlider, SpeedSlider);
    }

    public void SaveSettings(SettingsHandler settingsHandler)
    {
        settingsHandler.ActiveSettings.Channel = ChannelTextBoxText;
        settingsHandler.ActiveSettings.MusicEnabled = MusicEnabledCheckbox;
        settingsHandler.ActiveSettings.EffectsEnabled = EffectsEnabledCheckbox;
    }

    #endregion
}
