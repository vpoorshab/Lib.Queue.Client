using System;

namespace Lib.Queueing.Client
{
    public interface IPublisher
    {

        Guid PublishMessage<T>(T payload, PublishContext publishContext);
    }
}