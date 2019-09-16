using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lib.Queueing.Client.MessageControl
{
    /// <summary>
    /// State data about the message control event.
    /// </summary>
    public class StateData
    {


        #region "Properties"

        /// <summary>
        /// The type of data being represented in StateData.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public StateDataTypeList StateDataType { get; set; }

        /// <summary>
        /// The message being sent.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A code representing the message StateData.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The StateData detail.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Custom key value pair for handling addition information.
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> Keys { get; set; }

        /// <summary>
        /// The inner StateData object if there is a hierarchical relationship.
        /// </summary>
        public StateData InnerStateData { get; set; }

        #endregion

        #region "Methods"

        public override string ToString()
        {


            var dataAsString = new StringBuilder();
            dataAsString.AppendLine($"StateDataType: {StateDataType.ToString()}");
            dataAsString.AppendLine($"Code: {Code}");
            dataAsString.AppendLine($"Message: {Message}");
            dataAsString.AppendLine($"Detail: {Detail}");
            dataAsString.AppendLine($"Keys: {SerializeKeys()}");
            dataAsString.AppendLine($"-------------------------------------------------");

            if (InnerStateData != null)
            {
                dataAsString.AppendLine("--------------- Inner State Data ---------------");
                dataAsString.AppendLine(InnerStateData.ToString());
            }

            return dataAsString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static StateData GetExceptionStateData(Exception ex)
        {
            int hresult = 0;
            PropertyInfo propertyInfo = null;

            // NOTE: HResult is protected in .NET 4.0, public in .NET 4.5. This hack will make it
            // NOTE: work in both
            propertyInfo = typeof(Exception).GetProperty("HResult");
            hresult = (int)propertyInfo.GetValue(ex, null);

            StateData data = new StateData()
            {
                StateDataType = StateDataTypeList.Error,
                Code = hresult.ToString(),
                Message = ex.GetType().FullName,
                Detail = StateData.GetExceptionDetail(ex)
            };

            if (ex.InnerException != null)
            {
                data.InnerStateData = GetExceptionStateData(ex.InnerException);
            }

            return data;
        }

        /// <summary>
        /// Generate a detail string for message messageControl exceptions.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string GetExceptionDetail(Exception ex)
        {

            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"Exception : {ex}");
            return strBuilder.ToString();

        }

        /// <summary>
        /// Serializes the keys dictionary
        /// </summary>
        /// <returns>Serialized Keys dictionary</returns>
        private string SerializeKeys()
        {
            return JsonConvert.SerializeObject(Keys);
        }

        #endregion
    }
}