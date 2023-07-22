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

    private void NewIrcPrivMsg(IrcPrivMsg ircPrivMsg)
    {
        _textBoxLogger.WriteLine($"⌨ {ircPrivMsg.UserName}: {ircPrivMsg.Message}");

        if (ircPrivMsg.Bits == null)
            return;

        _textBoxLogger.WriteLine($"🔶 Bits: {ircPrivMsg.Bits} by {ircPrivMsg.UserName}");
        if (int.TryParse(ircPrivMsg.Bits, out int bits))
        {
            if (bits == _settingsHandler.ActiveSettings.Volume.Up)
            {
                _bitMusicViewModel.VolumeSlider =
                    _settingsHandler.ActiveSettings.Volume.GetNextStepValue(_bitMusicViewModel.VolumeSlider);
                _textBoxLogger.WriteLine($"📻 Volume up ({_bitMusicViewModel.VolumeSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Down)
            {
                _bitMusicViewModel.VolumeSlider =
                    _settingsHandler.ActiveSettings.Volume.GetPreviousStepValue(_bitMusicViewModel.VolumeSlider);
                _textBoxLogger.WriteLine($"📻 Volume down ({_bitMusicViewModel.VolumeSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Max)
            {
                _bitMusicViewModel.VolumeSlider = _settingsHandler.ActiveSettings.Volume.GetMaxStepValue();
                _textBoxLogger.WriteLine($"📻 Volume to max ({_bitMusicViewModel.VolumeSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Min)
            {
                _bitMusicViewModel.VolumeSlider = _settingsHandler.ActiveSettings.Volume.GetMinStepValue();
                _textBoxLogger.WriteLine($"📻 Volume to min ({_bitMusicViewModel.VolumeSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Up)
            {
                _bitMusicViewModel.SpeedSlider =
                    _settingsHandler.ActiveSettings.Speed.GetNextStepValue(_bitMusicViewModel.SpeedSlider);
                _textBoxLogger.WriteLine($"📻 Speed up ({_bitMusicViewModel.SpeedSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Down)
            {
                _bitMusicViewModel.SpeedSlider =
                    _settingsHandler.ActiveSettings.Speed.GetPreviousStepValue(_bitMusicViewModel.SpeedSlider);
                _textBoxLogger.WriteLine($"📻 Speed down to ({_bitMusicViewModel.SpeedSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Max)
            {
                _bitMusicViewModel.SpeedSlider = _settingsHandler.ActiveSettings.Speed.GetMaxStepValue();
                _textBoxLogger.WriteLine($"📻 Speed to max ({_bitMusicViewModel.SpeedSlider})");
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Min)
            {
                _bitMusicViewModel.SpeedSlider = _settingsHandler.ActiveSettings.Speed.GetMinStepValue();
                _textBoxLogger.WriteLine($"📻 Speed to min ({_bitMusicViewModel.SpeedSlider})");
            }
        }
    }

    #endregion
}
