using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.Queueing.Client.MessageControl
{
    /// <summary>
    /// The type of data being represented in StateData.
    /// </summary>
    public enum StateDataTypeList
    {
        Information,
        Error,
        Success
    }


    public enum MessageState
    {
        Sent,
        Received,
        Success,
        Retry,
        Failure
    }
}
