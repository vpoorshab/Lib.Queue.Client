using System;

namespace Lib.Queueing.Client.MessageControl
{
    public interface IMessageControlService
    {

        void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting);
        void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, StateResponse stateResponse);
        void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, Exception exception);
        void SendMessageControl(MessageState state, IMessage sourceMessage, IMessageQueueSetting messageQueueSetting, Exception exception, StateResponse stateResponse);


    }
}