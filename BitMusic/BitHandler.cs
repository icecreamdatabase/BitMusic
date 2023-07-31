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
                double oldVolume = _bitMusicViewModel.VolumeSlider;
                double newVolume = _settingsHandler.ActiveSettings.Volume.GetNextStepValue(oldVolume);
                
                _textBoxLogger.WriteLine($"📻 Volume up ({oldVolume} --> {newVolume})");
                _bitMusicViewModel.VolumeSlider = newVolume;
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Down)
            {
                double oldVolume = _bitMusicViewModel.VolumeSlider;
                double newVolume = _settingsHandler.ActiveSettings.Volume.GetPreviousStepValue(oldVolume);
                
                _textBoxLogger.WriteLine($"📻 Volume down ({oldVolume} --> {newVolume})");
                _bitMusicViewModel.VolumeSlider = newVolume;
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Max)
            {
                double oldVolume = _bitMusicViewModel.VolumeSlider;
                double newVolume = _settingsHandler.ActiveSettings.Volume.GetMaxStepValue();
                
                _textBoxLogger.WriteLine($"📻 Volume to max ({oldVolume} --> {newVolume})");
                _bitMusicViewModel.VolumeSlider = newVolume;
            }
            else if (bits == _settingsHandler.ActiveSettings.Volume.Min)
            {
                double oldVolume = _bitMusicViewModel.VolumeSlider;
                double newVolume = _settingsHandler.ActiveSettings.Volume.GetMinStepValue();
                
                _textBoxLogger.WriteLine($"📻 Volume to min ({oldVolume} --> {newVolume})");
                _bitMusicViewModel.VolumeSlider = newVolume;
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Up)
            {
                double oldSpeed = _bitMusicViewModel.SpeedSlider;
                double newSpeed = _settingsHandler.ActiveSettings.Speed.GetNextStepValue(oldSpeed);
                
                _textBoxLogger.WriteLine($"📻 Speed up ({oldSpeed} --> {newSpeed})");
                _bitMusicViewModel.SpeedSlider = newSpeed;
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Down)
            {
                double oldSpeed = _bitMusicViewModel.SpeedSlider;
                double newSpeed = _settingsHandler.ActiveSettings.Speed.GetPreviousStepValue(oldSpeed);
                
                _textBoxLogger.WriteLine($"📻 Speed down ({oldSpeed} --> {newSpeed})");
                _bitMusicViewModel.SpeedSlider = newSpeed;
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Max)
            {
                double oldSpeed = _bitMusicViewModel.SpeedSlider;
                double newSpeed = _settingsHandler.ActiveSettings.Speed.GetMaxStepValue();
                
                _textBoxLogger.WriteLine($"📻 Speed to max ({oldSpeed} --> {newSpeed})");
                _bitMusicViewModel.SpeedSlider = newSpeed;
            }
            else if (bits == _settingsHandler.ActiveSettings.Speed.Min)
            {
                double oldSpeed = _bitMusicViewModel.SpeedSlider;
                double newSpeed = _settingsHandler.ActiveSettings.Speed.GetMinStepValue();
                
                _textBoxLogger.WriteLine($"📻 Speed to min ({oldSpeed} --> {newSpeed})");
                _bitMusicViewModel.SpeedSlider = newSpeed;
            }
        }
    }

    #endregion
}
