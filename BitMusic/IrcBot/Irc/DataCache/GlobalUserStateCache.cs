using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;

namespace BitMusic.IrcBot.Irc.DataCache;

public class GlobalUserStateCache
{
    public IrcGlobalUserState? LastGlobalUserState { get; set; }
}
