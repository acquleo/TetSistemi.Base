using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;

namespace ICCS.DataBroker.MQTT
{
    public delegate void dClientMessageEvent(ITetMqttClient client, MessageArgs message);
    
    public delegate void dClientEvent(ITetMqttClient client);

    public interface ITetMqttClient: IEnabler, IDisposable
    {
        string ClientId { get; }
        string Host { get; }
        int Port { get; }
        bool IsConnected { get; }

        event dClientEvent Connected;

        event dClientEvent Disconnected;

        event dClientMessageEvent MessageReceived;

        //void Subscribe(List<TopicInfo> topics);
        Task SubscribeAsync(List<TopicInfo> topics);
        //bool Publish(TopicInfo topic, string message, bool retain);
        
        Task<bool> PublishAsync(TopicInfo topic, string message, bool retain, string contentType = null, string responseTopic = null, List<UserPropertyInfo> userPropertyInfo = null);
        Task<bool> PublishAsync(TopicInfo topic, byte[] message, bool retain, string contentType = null, string responseTopic = null, List<UserPropertyInfo> userPropertyInfo = null);
    }

    public class TopicInfo
    {
        public string Topic { get; set; } = string.Empty;


        public byte QosLevel { get; set; } = 0;

    }

    public class UserPropertyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class MessageArgs
    {
        public string ResponseTopic { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public string Topic { get; set; }=string.Empty;

        public string Message { get; set; } = string.Empty;

        public byte[] MessageRaw { get; set; }

        public bool DupFlag { get; set; } = false;

        public byte QosLevel { get; set; } = 0;

        public bool Retain { get; set; } = false;
    }
}
