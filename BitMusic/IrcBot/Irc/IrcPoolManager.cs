using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitMusic.IrcBot.Bot;
using BitMusic.IrcBot.Irc.DataCache;
using BitMusic.IrcBot.Irc.DataTypes;
using BitMusic.IrcBot.Irc.DataTypes.FromTwitch;
using BitMusic.IrcBot.Irc.DataTypes.ToTwitch;

namespace BitMusic.IrcBot.Irc;

public class IrcPoolManager
{
    private int _ircLastUsedSendClientIndex;
    private readonly List<IrcClient> _ircSendClients = new();
    private readonly List<IrcClient> _ircReceiveClients = new();
    private const int MaxChannelsPerIrcClient = 200;

    private readonly ConcurrentDictionary<string, string> _previousMessageInChannel = new();
    private readonly ConcurrentDictionary<string, DateTime> _previousMessageDateTimeInChannel = new();

    public string BotUsername => _botInstance.UserName;
    public string BotOauth => _botInstance.Token;

    private readonly BotInstance _botInstance;
    private readonly IrcSendQueue _ircSendQueue;

    public IrcBuckets IrcBuckets { get; }
    public UserStateCache UserStateCache { get; } = new();
    public GlobalUserStateCache GlobalUserStateCache { get; } = new();

    public IrcPoolManager(BotInstance botInstance)
    {
        _botInstance = botInstance;
        IrcBuckets = new IrcBuckets(_botInstance.Limits);
        _ircSendQueue = new IrcSendQueue(this);
        for (int i = 0; i < _botInstance.Limits.SendConnections; i++)
        {
            _ircSendClients.Add(
                new IrcClient(
                    this,
                    false,
                    $"{_botInstance.UserName}_send_{_ircSendClients.Count + 1}"
                )
            );
        }

        UpdateChannels();
    }

    public Task IntervalPing()
    {
        UpdateChannels();
        _ircReceiveClients.ForEach(c => c.CheckAlive());
        _ircSendClients.ForEach(c => c.CheckAlive());
        return Task.CompletedTask;
    }

    public void UpdateChannels()
    {
        SetChannel(_botInstance.Channels.ToArray());
    }

    public void SetChannel(params string[] channelNames)
    {
        Part(_ircReceiveClients.SelectMany(client => client.Channels).Except(channelNames).ToList());
        Join(channelNames.Except(_ircReceiveClients.SelectMany(client => client.Channels)).ToList());
    }

    private void Join(List<string> channels)
    {
        if (channels.Count == 0) return;
        foreach (IrcClient ircClient in _ircReceiveClients
                     .Where(ircClient => ircClient.Channels.Count < MaxChannelsPerIrcClient)
                )
        {
            int freeSlots = MaxChannelsPerIrcClient - ircClient.Channels.Count;
            List<string> newChannels = channels.Take(freeSlots).ToList();
            //BackgroundTask Failed System.ArgumentException: Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.
            channels.RemoveRange(0, Math.Min(freeSlots, channels.Count));
            ircClient.Channels.AddRange(newChannels);
        }

        // Need new IrcClient
        while (channels.Count > 0)
        {
            IrcClient ircClient = new IrcClient(
                this,
                false,
                $"{_botInstance.UserName}_receive_{_ircReceiveClients.Count + 1}"
            );
            _ircReceiveClients.Add(ircClient);

            List<string> newChannels = channels.Take(Math.Min(channels.Count, MaxChannelsPerIrcClient)).ToList();
            channels.RemoveRange(0, Math.Min(channels.Count, MaxChannelsPerIrcClient));
            ircClient.Channels.AddRange(newChannels);
        }
    }

    private void Part(IReadOnlyCollection<string> channelNames)
    {
        if (channelNames.Count == 0) return;

        foreach (string channelName in channelNames)
            GetIrcClientOfChannel(channelName)?.Channels.Remove(channelName);
    }

    public void SendMessage(PrivMsgToTwitch privMsg)
    {
        _ircSendQueue.Enqueue(privMsg);
        //foreach (PrivMsgToTwitch msg in morePrivMsgsInBatch)
        //{
        //    msg.UseSameSendConnectionAsPreviousMsg = true;
        //    _ircSendQueue.Enqueue(msg);
        //}
    }

    //@client-nonce=xxx;reply-parent-msg-id=xxx PRIVMSG #channel :xxxxxx `
    public async Task SendMessageNoQueue(PrivMsgToTwitch privMsgToTwitch)
    {
        bool useModRateLimit = UserStateCache.IsModInChannel(privMsgToTwitch.RoomName);

        /* ---------- Ratelimit ---------- */
        await IrcBuckets.WaitForMessageTicket(useModRateLimit);
        await HandleGlobalCooldown(privMsgToTwitch, useModRateLimit);

        /* ---------- Message adjustment ---------- */
        HandleMessageCleanup(privMsgToTwitch);
        HandleDuplicateMessage(privMsgToTwitch);

        /* ---------- Advance send client index if needed ---------- */
        if (!privMsgToTwitch.UseSameSendConnectionAsPreviousMsg)
            _ircLastUsedSendClientIndex = (_ircLastUsedSendClientIndex + 1) % _ircSendClients.Count;

        Console.WriteLine("{0} sending: {1}", _botInstance.UserName, privMsgToTwitch);

        /* ---------- Send message ---------- */
        await _ircSendClients[_ircLastUsedSendClientIndex].SendLine(privMsgToTwitch.ToString());
    }

    private async Task HandleGlobalCooldown(PrivMsgToTwitch privMsgToTwitch, bool useModRateLimit)
    {
        if (useModRateLimit) return;

        if (_previousMessageDateTimeInChannel.TryGetValue(privMsgToTwitch.RoomName, out DateTime prevMessageDateTime))
        {
            double msSinceLastMessageInSameChannel = (DateTime.UtcNow - prevMessageDateTime).TotalMilliseconds;

            double additionalWaitRequired = 1100 - msSinceLastMessageInSameChannel;

            if (additionalWaitRequired > 0)
            {
                Console.WriteLine("Sending messages too fast. Waiting for {0} ms", (int)additionalWaitRequired);
                await Task.Delay((int)additionalWaitRequired, CancellationToken.None);
            }
        }

        _previousMessageDateTimeInChannel[privMsgToTwitch.RoomName] = DateTime.UtcNow;
    }

    private static void HandleMessageCleanup(PrivMsgToTwitch privMsgToTwitch)
    {
        privMsgToTwitch.Message = privMsgToTwitch.Message.Trim();
    }

    private void HandleDuplicateMessage(PrivMsgToTwitch privMsgToTwitch)
    {
        // Is the message identical
        if (_previousMessageInChannel.ContainsKey(privMsgToTwitch.RoomName) &&
            privMsgToTwitch.Message == _previousMessageInChannel[privMsgToTwitch.RoomName]
           )
        {
            // If the message is an ACTION we want to ignore the first space after the .me /me
            // for the startIndex we technically need to check for index out of bound.
            // But we always trim beforehand and therefore the space can never be the last character in the string.
            int spaceIndex = privMsgToTwitch.Message.StartsWith(".me ") || privMsgToTwitch.Message.StartsWith("/me ")
                ? privMsgToTwitch.Message.IndexOf(' ', privMsgToTwitch.Message.IndexOf(' ') + 1)
                : privMsgToTwitch.Message.IndexOf(' ');
            if (spaceIndex == -1)
                // No space found, fall back to the old magic character.
                privMsgToTwitch.Message += " \U000E0000";
            else
                // Insert a second space at the position of the first space.
                privMsgToTwitch.Message = privMsgToTwitch.Message.Insert(spaceIndex, " ");
        }

        _previousMessageInChannel[privMsgToTwitch.RoomName] = privMsgToTwitch.Message;
    }

    private IrcClient? GetIrcClientOfChannel(string channel)
    {
        return _ircReceiveClients.FirstOrDefault(client => client.Channels.Contains(channel));
    }

    public Task ForceCheckAuth()
    {
        return Task.CompletedTask;
    }

    public void NewIrcMessage(IrcMessage ircMessage)
    {
        switch (ircMessage.IrcCommand)
        {
            case IrcCommands.ClearChat:
            {
                IrcClearChat ircClearChat = new IrcClearChat(ircMessage);
                _botInstance.NewIrcClearChat(ircClearChat);
                break;
            }
            case IrcCommands.ClearMsg:
            {
                IrcClearMsg ircClearMsg = new IrcClearMsg(ircMessage);
                _botInstance.NewIrcClearMsg(ircClearMsg);
                break;
            }
            case IrcCommands.GlobalUserState:
            {
                IrcGlobalUserState ircGlobalUserState = new IrcGlobalUserState(ircMessage);
                GlobalUserStateCache.LastGlobalUserState = ircGlobalUserState;
                _botInstance.NewIrcGlobalUserState(ircGlobalUserState);
                break;
            }
            case IrcCommands.HostTarget:
            {
                IrcHostTarget ircHostTarget = new IrcHostTarget(ircMessage);
                _botInstance.NewIrcHostTarget(ircHostTarget);
                break;
            }
            case IrcCommands.Notice:
            {
                IrcNotice ircNotice = new IrcNotice(ircMessage);

                if (ircNotice.MessageId is
                    NoticeMessageId.MsgBanned or
                    NoticeMessageId.MsgChannelSuspended or
                    NoticeMessageId.MsgChannelBlocked or
                    NoticeMessageId.TosBan
                   )
                {
                    Console.WriteLine("Bot {0} failed joining {1}: {2}",
                        _botInstance.UserName,
                        ircNotice.RoomName,
                        Enum.GetName(ircNotice.MessageId)
                    );
                    UpdateChannels();
                }

                _botInstance.NewIrcNotice(ircNotice);
                break;
            }
            case IrcCommands.PrivMsg:
            {
                //_logger.LogInformation("{Raw}", ircMessage.RawSource);
                IrcPrivMsg ircPrivMsg = new IrcPrivMsg(ircMessage);
                //_logger.LogInformation("{Command}: {Channel}: {Msg}", ircMessage.IrcCommand, ircPrivMsg.RoomName, ircPrivMsg.Message);
                _botInstance.NewIrcPrivMsg(ircPrivMsg);
                break;
            }
            case IrcCommands.RoomState:
            {
                IrcRoomState ircRoomState = new IrcRoomState(ircMessage);
                _botInstance.NewIrcRoomState(ircRoomState);
                break;
            }
            case IrcCommands.UserNotice:
            {
                IrcUserNotice ircUserNotice = new IrcUserNotice(ircMessage);
                _botInstance.NewIrcUserNotice(ircUserNotice);
                break;
            }
            case IrcCommands.UserState:
            {
                IrcUserState ircUserState = new IrcUserState(ircMessage);
                UserStateCache.AddUserState(ircUserState);
                _botInstance.NewIrcUserState(ircUserState);
                break;
            }
            case (IrcCommands.RplNamReply):
            {
                _botInstance.NewIrcNamReply(ircMessage.IrcParameters[2][1..], ircMessage.IrcParameters[3]);
                break;
            }
            default:
            {
                Console.WriteLine("{0}: {1}", ircMessage.IrcCommand, ircMessage.RawSource);
                break;
            }
        }
    }

    public void RemoveReceiveClient(IrcClient ircClient)
    {
        _ircReceiveClients.Remove(ircClient);
    }
}
