using BitMusic.Helper;
using BitMusic.IrcBot.Bot;
using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;
using BitMusic.Settings;

namespace BitMusic;

public class BitHandler
{
    private readonly TextBoxLogger _textBoxLogger;
    private readonly SettingsHandler _settingsHandler;
    private readonly BotInstance _botInstance;

    public BitHandler(TextBoxLogger textBoxLogger, SettingsHandler settingsHandler, BotInstance botInstance)
    {
        _textBoxLogger = textBoxLogger;
        _settingsHandler = settingsHandler;
        _botInstance = botInstance;

        _botInstance.OnNewIrcPrivMsg += NewIrcPrivMsg;
    }

    #region Methods

    private void NewIrcPrivMsg(IrcPrivMsg ircPrivMsg)
    {
        _textBoxLogger.WriteLine($"{ircPrivMsg.RoomName}: {ircPrivMsg.Message}");

        if(ircPrivMsg.Bits != null)
            _textBoxLogger.WriteLine($"Bits: {ircPrivMsg.Bits}");
    }

    #endregion
}
