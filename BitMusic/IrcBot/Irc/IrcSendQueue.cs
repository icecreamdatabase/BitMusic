using System.Collections.Concurrent;
using System.Threading.Tasks;
using BitMusic.IrcBot.Irc.DataTypes.ToTwitch;

namespace BitMusic.IrcBot.Irc;

public class IrcSendQueue
{
    private readonly IrcPoolManager _ircPoolManager;
    private readonly ConcurrentQueue<PrivMsgToTwitch> _queue = new();

    private Task _currentCheckQueueTask = Task.CompletedTask;

    public IrcSendQueue(IrcPoolManager ircPoolManager)
    {
        _ircPoolManager = ircPoolManager;
    }

    public void Enqueue(PrivMsgToTwitch privMsgToTwitch)
    {
        _queue.Enqueue(privMsgToTwitch);
        if (_currentCheckQueueTask.IsCompleted)
        {
            _currentCheckQueueTask = Task.Run(CheckQueue);
        }
    }

    private async Task CheckQueue()
    {
        while (_queue.TryDequeue(out PrivMsgToTwitch? privMsgToTwitch))
            await _ircPoolManager.SendMessageNoQueue(privMsgToTwitch);
    }
}
