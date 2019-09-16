namespace Lib.Queueing.Client
{
    public interface IMessageQueueSetting
    {
        string SystemIdentifier { get; }

        string EventType { get; }

        string Exchange { get; }

        string Queue { get; }
    }
}