using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ICCS.DataBroker.EventSource
{

    public delegate void dDataBrokerEventSourceMessageReceiveRaw(IDataBrokerEventSourceRaw source,string topic,bool retain, string message, string responsetopic
        , string contenttype, byte qos);
    public delegate void dDataBrokerEventSourceRaw(IDataBrokerEventSourceRaw source);
    public delegate void dDataBrokerOnMessageRequestRaw(IDataBrokerEventSourceRaw source, HttpRequestMessage request);

    public interface IDataBrokerEventSourceRaw
    {
        event dDataBrokerEventSourceMessageReceiveRaw MessageReceive;
        event dDataBrokerEventSourceRaw SseConnected;
        event dDataBrokerEventSourceRaw SseDisconnected;
        event dDataBrokerEventSourceRaw MqttConnected;
        event dDataBrokerEventSourceRaw MqttDisconnected;
        event dDataBrokerOnMessageRequestRaw OnHttpMessageRequest;
        bool SseIsConnected { get; }
        bool MqttIsConnected { get; }
    }
}
