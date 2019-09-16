using System;
using Lib.Queueing.Client.MessageControl;

namespace Lib.Queueing.Client
{
    public class MessageProcessorResult
    {
        public ResultStatus Status { get; protected set; }

        public StateResponse Response { get; protected set; }


        #region Success
        public static MessageProcessorResult Success()
        {

            return new MessageProcessorResult() { Status = ResultStatus.Success };
        }


        public static MessageProcessorResult Success(StateResponse response)
        {

            return new MessageProcessorResult() { Response = response, Status = ResultStatus.Success };
        }


        public static MessageProcessorResult Success(string message)
        {

            return new MessageProcessorResult() { Response = new StateResponse(new StateData() { Message = message, StateDataType = StateDataTypeList.Success }), Status = ResultStatus.Success };
        }
        #endregion



        public static MessageProcessorResult Fail()
        {

            return new MessageProcessorResult() { Status = ResultStatus.Failure };
        }

        public static MessageProcessorResult Fail(string message)
        {

            return new MessageProcessorResult()
            {
                Response = new StateResponse(new StateData()
                {
                    Message = message,
                    StateDataType = StateDataTypeList.Error
                })
                {
                    IsSuccess = false,
                    IsRecoverable = false
                }
                ,
                Status = ResultStatus.Failure
            };
        }

        public static MessageProcessorResult Fail(Exception failure)
        {

            return new MessageProcessorResult()
            {
                Response = new StateResponse(StateData.GetExceptionStateData(failure))
                ,
                Status = ResultStatus.Failure
            };
        }


        public static MessageProcessorResult Fail(StateResponse response)
        {
            response.IsSuccess = false;
            return new MessageProcessorResult()
            {
                Response = response,
                Status = ResultStatus.Failure
            };
        }
        public static MessageProcessorResult Retry()
        {

            return new MessageProcessorResult()
            {
                Status = ResultStatus.Retry
            };
        }

        public static MessageProcessorResult Retry(string message)
        {

            return new MessageProcessorResult()
            {
                Response = new StateResponse(new StateData()
                {
                    Message = message,
                    StateDataType = StateDataTypeList.Information,

                })
                {
                    IsSuccess = false,
                    IsRecoverable = true
                },
                Status = ResultStatus.Retry
            };
        }

        public static MessageProcessorResult Retry(StateResponse response)
        {
            response.IsRecoverable = true;
            return new MessageProcessorResult()
            {
                Response = response,
                Status = ResultStatus.Retry
            };
        }



    }
}