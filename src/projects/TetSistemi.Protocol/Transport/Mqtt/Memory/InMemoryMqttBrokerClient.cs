using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using static TetSistemi.Protocol.Transport.Mqtt.Memory.InMemoryMqttBroker;

namespace TetSistemi.Protocol.Transport.Mqtt.Memory
{
    public class InMemoryMqttBrokerClient : IMqttDataTransport, IEnabler, IDisposable
    {
        List<TopicInfo> topics;
        public InMemoryMqttBrokerClient(List<TopicInfo> topics)
        {
            this.topics = topics;
        }

        void Subscribe()
        {

        }

        public bool Available => true;

        public event dDataReceivedAsync<MqttMessage> DataReceivedAsync;
        public event dDataTransportAsync<MqttMessage> DataTransportAvailable;
        public event dDataTransportAsync<MqttMessage> DataTransportUnavailable;
        public event dDataTransportTraceAsync<MqttMessage> DataTransportTraceAsync;

        public DataEndpoint GetRemoteEndpoint()
        {
            return new EmptyDataEndpoint();
        }

        public Task<bool> SendAsync(MqttMessage data)
        {
            return SendAsync(data, new EmptyDataEndpoint());
        }

        public Task<bool> SendAsync(MqttMessage data, DataEndpoint to)
        {
            InMemoryMqttBrokerMessage retmsg = new InMemoryMqttBrokerMessage()
            {
                ContentType = data.ContentType,
                message = data.PayloadRaw,
                ResponseTopic = data.Responsetopic,
                Retain = data.Retain,
                topic = new TopicInfo()
                {
                    QosLevel = data.Qos,
                    Topic = data.Topic
                },
                UserProperties = data.Userproperties
            };


            InMemoryMqttBroker.Instance.Publish(retmsg);

            return Task.FromResult(true);
        }

        public void Enable()
        {
            InMemoryMqttBroker.Instance.Subscribe(this, topics);
        }

        public void Disable()
        {
            
        }

        public bool IsEnabled()
        {
            return true;
        }

        public async void PushMessage(InMemoryMqttBrokerMessage msg)
        {
            MqttMessage retmsg = new MqttMessage()
            {
                ContentType = msg.ContentType,
                Payload = string.Empty,
                PayloadRaw = msg.message,
                Qos = msg.topic.QosLevel,
                Responsetopic = msg.ResponseTopic,
                Retain = msg.Retain,
                Topic = msg.topic.Topic,
                Userproperties = msg.UserProperties
            };

            await Task.WhenAll(Array.ConvertAll(
                    DataReceivedAsync.GetInvocationList(),
                    e => ((dDataReceivedAsync<MqttMessage>)e)(this, retmsg, new EmptyDataEndpoint()))).ConfigureAwait(false);
        }

        public void Dispose()
        {
            
        }

        public Task<bool> SubscribeAsync(List<MqttTopic> subscribe)
        {            
            List<TopicInfo> topicInfos = new List<TopicInfo>();
            foreach (var intopic in subscribe)
            {
                topicInfos.Add(new TopicInfo() { QosLevel = intopic.Qos, Topic = intopic.Topic });
            }

            InMemoryMqttBroker.Instance.Subscribe(this, topics);

            return Task.FromResult(true);
        }
    }
}
