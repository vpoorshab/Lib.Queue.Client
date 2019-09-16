using System.Threading.Tasks;

namespace Lib.Queueing.Client
{
    public interface IMessageProcessor
    {
        Task<MessageProcessorResult> ProcessAsync(IMessage message);
    }
}