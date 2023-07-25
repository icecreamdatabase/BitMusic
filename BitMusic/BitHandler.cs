using System.Windows;
using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;
using BitMusic.Settings;
using BitMusic.ViewModel;

namespace BitMusic;

public class BitHandler
{
    private readonly BitMusicViewModel _bitMusicViewModel;
    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly BotInstance _botInstance;


    public BitHandler(BitMusicViewModel bitMusicViewModel, TextBoxLogger textBoxLogger, BotInstance botInstance,
        SettingsHandler settingsHandler)
    {
        _bitMusicViewModel = bitMusicViewModel;
        _textBoxLogger = textBoxLogger;
        _settingsHandler = settingsHandler;
        _botInstance = botInstance;

        _botInstance.OnNewIrcPrivMsg += NewIrcPrivMsg;
    }


    #region Methods

    private void NewIrcPrivMsg(IrcPrivMsg ircPrivMsg) => Application.Current.Dispatcher.Invoke(HandleBits, ircPrivMsg);

    private void HandleBits(IrcPrivMsg ircPrivMsg)
    {
        _textBoxLogger.WriteLine($"⌨ {ircPrivMsg.UserName}: {ircPrivMsg.Message}");

        if (ircPrivMsg.Bits == null)
            return;

        _textBoxLogger.WriteLine($"🔶 Bits: {ircPrivMsg.Bits} by {ircPrivMsg.UserName}");
        if (int.TryParse(ircPrivMsg.Bits, out int bits))
        {
            if (bits == _settingsHandler.ActiveSettings.Skip)
            {
                _textBoxLogger.WriteLine("📻 Skipping song");
                _bitMusicViewModel.SkipSong();
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Up)
            {
                _textBoxLogger.WriteLine($"📻 Volume up ({_bitMusicViewModel.VolumeSlider})");
                _bitMusicViewModel.VolumeSlider =
                    _settingsHandler.ActiveSettings.Volume.GetNextStepValue(_bitMusicViewModel.VolumeSlider);
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Down)
            {
                _textBoxLogger.WriteLine($"📻 Volume down ({_bitMusicViewModel.VolumeSlider})");
                _bitMusicViewModel.VolumeSlider =
                    _settingsHandler.ActiveSettings.Volume.GetPreviousStepValue(_bitMusicViewModel.VolumeSlider);
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Max)
            {
                _textBoxLogger.WriteLine($"📻 Volume to max ({_bitMusicViewModel.VolumeSlider})");
                _bitMusicViewModel.VolumeSlider = _settingsHandler.ActiveSettings.Volume.GetMaxStepValue();
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Min)
            {
                _textBoxLogger.WriteLine($"📻 Volume to min ({_bitMusicViewModel.VolumeSlider})");
                _bitMusicViewModel.VolumeSlider = _settingsHandler.ActiveSettings.Volume.GetMinStepValue();
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Up)
            {
                _textBoxLogger.WriteLine($"📻 Speed up ({_bitMusicViewModel.SpeedSlider})");
                _bitMusicViewModel.SpeedSlider =
                    _settingsHandler.ActiveSettings.Speed.GetNextStepValue(_bitMusicViewModel.SpeedSlider);
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Down)
            {
                _textBoxLogger.WriteLine($"📻 Speed down to ({_bitMusicViewModel.SpeedSlider})");
                _bitMusicViewModel.SpeedSlider =
                    _settingsHandler.ActiveSettings.Speed.GetPreviousStepValue(_bitMusicViewModel.SpeedSlider);
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Max)
            {
                _textBoxLogger.WriteLine($"📻 Speed to max ({_bitMusicViewModel.SpeedSlider})");
                _bitMusicViewModel.SpeedSlider = _settingsHandler.ActiveSettings.Speed.GetMaxStepValue();
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Min)
            {
                _textBoxLogger.WriteLine($"📻 Speed to min ({_bitMusicViewModel.SpeedSlider})");
                _bitMusicViewModel.SpeedSlider = _settingsHandler.ActiveSettings.Speed.GetMinStepValue();
            }
        }
    }

    #endregion
}
