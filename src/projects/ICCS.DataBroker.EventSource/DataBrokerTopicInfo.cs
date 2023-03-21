using System;

namespace ICCS.DataBroker.EventSource
{
    public class DataBrokerTopicInfo
    {
        public string Topic { get; set; }
        public byte Qos { get; set; }
    }
}
