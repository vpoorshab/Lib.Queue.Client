using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.Queueing.Client.MessageControl
{
    /// <summary>
    /// A common state class to return success, or information on any operation in the system.
    /// </summary>
    public class StateResponse
    {
        #region "Variables"

        #endregion

        #region "Constructors"

        public StateResponse() { }

        public StateResponse(StateData stateData)
        {
            if (stateData.StateDataType == StateDataTypeList.Error)
            {
                IsSuccess = false;
            }

            StateData.Add(stateData);
        }

        public StateResponse(List<StateData> stateData)
        {
            foreach (StateData data in stateData)
            {
                if (data.StateDataType == StateDataTypeList.Error)
                {
                    IsSuccess = false;
                }

                StateData.Add(data);
            }
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Indicates whether or not the operation was successful
        /// </summary>
        public bool IsSuccess
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Indicates whether or not the error can be retried with the same data
        /// </summary>
        public bool IsRecoverable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// A collection of messages
        /// </summary>
        public List<StateData> StateData
        {
            get;
            private set;
        } = new List<StateData>();

        /// <summary>
        /// An array generated from the StateData list. This is to help Kibana index the state data.
        /// </summary>
        public string[] StateDataCodes
        {
            get
            {
                List<string> list = new List<string>();

                foreach (StateData data in StateData)
                {
                    list.Add(data.Code);
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// An array generated from the StateData list. This is to help Kibana index the state data.
        /// </summary>
        public string[] StateDataMessages
        {
            get
            {
                List<string> list = new List<string>();

                foreach (StateData data in StateData)
                {
                    list.Add(data.Message);
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// An array generated from the StateData list. This is to help Kibana index the state data.
        /// </summary>
        public string[] StateDataDetails
        {
            get
            {
                List<string> list = new List<string>();

                foreach (StateData data in StateData)
                {
                    list.Add(data.Detail);
                }

                return list.ToArray();
            }
        }

        #endregion

        #region "Methods"

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine($"--------------- State Response -----------------");
            strBuilder.AppendLine($"IsSuccess: {IsSuccess}");
            strBuilder.AppendLine($"IsRecoverable: {IsRecoverable}");
            
           

            if (StateData != null && StateData.Count > 0)
            {
                strBuilder.AppendLine("--------------- State Data ---------------------");
            }

            foreach (StateData stateData in StateData)
            {
                strBuilder.AppendLine(stateData.ToString());
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Join two StateResponse objects.
        /// </summary>
        /// <param name="stateResponse"></param>
        public void Join(StateResponse stateResponse)
        {
            IsSuccess &= stateResponse.IsSuccess;
            IsRecoverable &= stateResponse.IsRecoverable;

            StateData = StateData.Union(stateResponse.StateData).ToList();
        }

        #endregion
    }
}