using System;
using System.Collections.Specialized;
using System.Timers;
using BitMusic.IrcBot.Helper;
using BitMusic.IrcBot.Irc;
using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;
using BitMusic.IrcBot.Irc.DataTypes.ToTwitch;
using Timer = System.Timers.Timer;

namespace BitMusic.IrcBot.Bot;

public class BotInstance : IDisposable
{
    private readonly IrcPoolManager _ircPoolManager;

    public string UserName { get; }
    public string Token { get; }
    public Limits Limits { get; private set; } = Limits.NormalBot;
    public readonly BulkObservableCollection<string> Channels = new();

    private readonly Timer _intervalTimer;

    private void OnChannelsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _ircPoolManager.UpdateChannels();
    }

    public BotInstance(string userName, string token)
    {
        UserName = userName;
        Token = token;

        if (UserName.ToLowerInvariant().StartsWith("justinfan"))
            Limits = Limits.AnonConnection;
                
        _ircPoolManager = new IrcPoolManager(this);
        Channels.CollectionChanged += OnChannelsChanged;

        _intervalTimer = new Timer(15000 /*ms*/);
        _intervalTimer.Elapsed += IntervalPing;
        _intervalTimer.AutoReset = true;
        _intervalTimer.Enabled = true;
    }

    private async void IntervalPing(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        await _ircPoolManager.IntervalPing();
    }

    public void SendPrivMsg(PrivMsgToTwitch privMsg)
    {
        _ircPoolManager.SendMessage(privMsg);
    }

    public IrcGlobalUserState? GetGlobalUserState() => _ircPoolManager.GlobalUserStateCache.LastGlobalUserState;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        //TODO
    }

    #region events

    public delegate void IrcClearChatEvent(IrcClearChat ircClearChat);

    public event IrcClearChatEvent? OnNewIrcClearChat;

    internal void NewIrcClearChat(IrcClearChat ircClearChat) => OnNewIrcClearChat?.Invoke(ircClearChat);


    public delegate void IrcClearMsgEvent(IrcClearMsg ircClearMsg);

    public event IrcClearMsgEvent? OnNewIrcClearMsg;

    internal void NewIrcClearMsg(IrcClearMsg ircClearMsg) => OnNewIrcClearMsg?.Invoke(ircClearMsg);


    public delegate void IrcGlobalUserStateEvent(IrcGlobalUserState ircGlobalUserState);

    public event IrcGlobalUserStateEvent? OnNewIrcGlobalUserState;

    internal void NewIrcGlobalUserState(IrcGlobalUserState ircGlobalUserState) =>
        OnNewIrcGlobalUserState?.Invoke(ircGlobalUserState);


    public delegate void IrcHostTargetEvent(IrcHostTarget ircHostTarget);

    public event IrcHostTargetEvent? OnNewIrcHostTarget;

    internal void NewIrcHostTarget(IrcHostTarget ircHostTarget) => OnNewIrcHostTarget?.Invoke(ircHostTarget);


    public delegate void IrcNoticeEvent(IrcNotice ircNotice);

    public event IrcNoticeEvent? OnNewIrcNotice;

    internal void NewIrcNotice(IrcNotice ircNotice) => OnNewIrcNotice?.Invoke(ircNotice);


    public delegate void IrcPrivMsgEvent(IrcPrivMsg ircPrivMsg);

    public event IrcPrivMsgEvent? OnNewIrcPrivMsg;

    internal void NewIrcPrivMsg(IrcPrivMsg ircPrivMsg) => OnNewIrcPrivMsg?.Invoke(ircPrivMsg);


    public delegate void IrcRoomStateEvent(IrcRoomState ircRoomState);

    public event IrcRoomStateEvent? OnNewIrcRoomState;

    internal void NewIrcRoomState(IrcRoomState ircRoomState) => OnNewIrcRoomState?.Invoke(ircRoomState);


    public delegate void IrcUserNoticeEvent(IrcUserNotice ircUserNotice);

    public event IrcUserNoticeEvent? OnNewIrcUserNotice;

    internal void NewIrcUserNotice(IrcUserNotice ircUserNotice) => OnNewIrcUserNotice?.Invoke(ircUserNotice);


    public delegate void IrcUserStateEvent(IrcUserState ircUserState);

    public event IrcUserStateEvent? OnNewIrcUserState;

    internal void NewIrcUserState(IrcUserState ircUserState) => OnNewIrcUserState?.Invoke(ircUserState);


    public delegate void IrcNamReply(string room, string userName);

    public event IrcNamReply? OnNewIrcNamReply;

    internal void NewIrcNamReply(string roomName, string userName) => OnNewIrcNamReply?.Invoke(roomName, userName);

    #endregion
}
