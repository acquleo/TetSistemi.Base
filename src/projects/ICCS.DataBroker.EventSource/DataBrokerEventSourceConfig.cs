using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ICCS.DataBroker.EventSource
{
    public class DataBrokerEventSourceConfig
    {
        public string BaseUrl { get; set; }
        public HttpMessageHandler CustomHttpMessageHandler { get; set; }
        public TimeSpan KeepAliveTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public List<DataBrokerTopicInfo> Topics { get; set; } = new List<DataBrokerTopicInfo>();
    }
}
