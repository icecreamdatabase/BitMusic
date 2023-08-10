using System.Threading;
using System.Windows;
using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;
using BitMusic.Settings;
using BitMusic.TMEffects;
using BitMusic.TMEffects.EffectTypes;
using BitMusic.ViewModel;

namespace BitMusic;

public class BitHandler
{
    #region Properties and Fields

    private readonly BitMusicViewModel _bitMusicViewModel;
    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly BotInstance _botInstance;

    #endregion

    #region Constructor

    public BitHandler(BitMusicViewModel bitMusicViewModel, TextBoxLogger textBoxLogger, BotInstance botInstance,
        SettingsHandler settingsHandler)
    {
        _bitMusicViewModel = bitMusicViewModel;
        _textBoxLogger = textBoxLogger;
        _settingsHandler = settingsHandler;
        _botInstance = botInstance;

        _botInstance.OnNewIrcPrivMsg += NewIrcPrivMsg;
    }

    #endregion

    #region Methods

    private void NewIrcPrivMsg(IrcPrivMsg ircPrivMsg) => Application.Current.Dispatcher.Invoke(HandleBits, ircPrivMsg);

    private void HandleBits(IrcPrivMsg ircPrivMsg)
    {
        _textBoxLogger.WriteLine($"⌨ {ircPrivMsg.UserName}: {ircPrivMsg.Message}");

        if (ircPrivMsg.UserId == 38949074 && ircPrivMsg.Message.StartsWith("!tmtest"))
        {
            Thread.Sleep(10000);
            TmEffectsHandling(_settingsHandler.ActiveSettings.TmSettings.BitAmount);
        }

        if (ircPrivMsg.Bits == null)
            return;

        _textBoxLogger.WriteLine($"🔶 Bits: {ircPrivMsg.Bits} by {ircPrivMsg.UserName}");

        if (!int.TryParse(ircPrivMsg.Bits, out int bits))
            return;

        TmEffectsHandling(bits);
        MusicHandling(bits);
    }

    private void TmEffectsHandling(int bits)
    {
        if (!_bitMusicViewModel.MainTabViewModel.EffectsEnabledCheckbox)
            return;

        if (bits != _settingsHandler.ActiveSettings.TmSettings.BitAmount)
            return;

        string processName = _settingsHandler.ActiveSettings.TmSettings.ProcessName;
        EffectBase? effect = _bitMusicViewModel.TmEffectsViewModel.EffectList.ExecuteRandomEffectByWeight(processName);
        if (effect != null)
            _textBoxLogger.WriteLine(effect.GetConsoleOutput());
        else
            _textBoxLogger.WriteLine("📻 No TM Keybinds defined");
    }

    private void MusicHandling(int bits)
    {
        if (!_bitMusicViewModel.MainTabViewModel.MusicEnabledCheckbox)
            return;

        if (bits == _settingsHandler.ActiveSettings.Skip)
        {
            _textBoxLogger.WriteLine("📻 Skipping song");
            _bitMusicViewModel.MainTabViewModel.SkipSong();
        }
        else if (bits == _settingsHandler.ActiveSettings.Volume.Up)
        {
            double oldVolume = _bitMusicViewModel.MainTabViewModel.VolumeSlider;
            double newVolume = _settingsHandler.ActiveSettings.Volume.GetNextStepValue(oldVolume);

            _textBoxLogger.WriteLine($"📻 Volume up ({oldVolume} --> {newVolume})");
            _bitMusicViewModel.MainTabViewModel.VolumeSlider = newVolume;
        }
        else if (bits == _settingsHandler.ActiveSettings.Volume.Down)
        {
            double oldVolume = _bitMusicViewModel.MainTabViewModel.VolumeSlider;
            double newVolume = _settingsHandler.ActiveSettings.Volume.GetPreviousStepValue(oldVolume);

            _textBoxLogger.WriteLine($"📻 Volume down ({oldVolume} --> {newVolume})");
            _bitMusicViewModel.MainTabViewModel.VolumeSlider = newVolume;
        }
        else if (bits == _settingsHandler.ActiveSettings.Volume.Max)
        {
            double oldVolume = _bitMusicViewModel.MainTabViewModel.VolumeSlider;
            double newVolume = _settingsHandler.ActiveSettings.Volume.GetMaxStepValue();

            _textBoxLogger.WriteLine($"📻 Volume to max ({oldVolume} --> {newVolume})");
            _bitMusicViewModel.MainTabViewModel.VolumeSlider = newVolume;
        }
        else if (bits == _settingsHandler.ActiveSettings.Volume.Min)
        {
            double oldVolume = _bitMusicViewModel.MainTabViewModel.VolumeSlider;
            double newVolume = _settingsHandler.ActiveSettings.Volume.GetMinStepValue();

            _textBoxLogger.WriteLine($"📻 Volume to min ({oldVolume} --> {newVolume})");
            _bitMusicViewModel.MainTabViewModel.VolumeSlider = newVolume;
        }
        else if (bits == _settingsHandler.ActiveSettings.Speed.Up)
        {
            double oldSpeed = _bitMusicViewModel.MainTabViewModel.SpeedSlider;
            double newSpeed = _settingsHandler.ActiveSettings.Speed.GetNextStepValue(oldSpeed);

            _textBoxLogger.WriteLine($"📻 Speed up ({oldSpeed} --> {newSpeed})");
            _bitMusicViewModel.MainTabViewModel.SpeedSlider = newSpeed;
        }
        else if (bits == _settingsHandler.ActiveSettings.Speed.Down)
        {
            double oldSpeed = _bitMusicViewModel.MainTabViewModel.SpeedSlider;
            double newSpeed = _settingsHandler.ActiveSettings.Speed.GetPreviousStepValue(oldSpeed);

            _textBoxLogger.WriteLine($"📻 Speed down ({oldSpeed} --> {newSpeed})");
            _bitMusicViewModel.MainTabViewModel.SpeedSlider = newSpeed;
        }
        else if (bits == _settingsHandler.ActiveSettings.Speed.Max)
        {
            double oldSpeed = _bitMusicViewModel.MainTabViewModel.SpeedSlider;
            double newSpeed = _settingsHandler.ActiveSettings.Speed.GetMaxStepValue();

            _textBoxLogger.WriteLine($"📻 Speed to max ({oldSpeed} --> {newSpeed})");
            _bitMusicViewModel.MainTabViewModel.SpeedSlider = newSpeed;
        }
        else if (bits == _settingsHandler.ActiveSettings.Speed.Min)
        {
            double oldSpeed = _bitMusicViewModel.MainTabViewModel.SpeedSlider;
            double newSpeed = _settingsHandler.ActiveSettings.Speed.GetMinStepValue();

            _textBoxLogger.WriteLine($"📻 Speed to min ({oldSpeed} --> {newSpeed})");
            _bitMusicViewModel.MainTabViewModel.SpeedSlider = newSpeed;
        }
    }

    #endregion
}
