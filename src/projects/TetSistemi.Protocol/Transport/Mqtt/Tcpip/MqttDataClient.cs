using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using ICCS.DataBroker.MQTT;
using System.Collections.Generic;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;

namespace TetSistemi.Protocol.Transport.Mqtt.Tcpip
{

    public class MqttDataClient : IMqttDataTransport, IEnabler, IDisposable
    {
        ICCS.DataBroker.MQTT.TetMqttClient client;

        List<TopicInfo> topics;

        /// <summary>
        /// 
        /// </summary>
        public event dDataReceivedAsync<MqttMessage> DataReceivedAsync;
        public event dDataTransportAsync<MqttMessage> DataTransportAvailable;
        public event dDataTransportAsync<MqttMessage> DataTransportUnavailable;
        public event dDataTransportTraceAsync<MqttMessage> DataTransportTraceAsync;
        public bool Available => client.IsConnected;

        public MqttDataClient(ICCS.DataBroker.MQTT.TetMqttClientConfig cfg, List<TopicInfo> topics)
        {
            this.client = new ICCS.DataBroker.MQTT.TetMqttClient(cfg);
            this.client.Connected += Client_Connected;
            this.client.Disconnected += Client_Disconnected;
            this.client.MessageReceived += Client_MessageReceived;
            this.topics = topics;
        }

        async void dequeue(ICCS.DataBroker.MQTT.MessageArgs message)
        {
            MqttMessage msg = new MqttMessage()
            {
                Payload = message.Message,
                Qos = message.QosLevel,
                Responsetopic = message.ResponseTopic,
                Retain = message.Retain,
                Topic = message.Topic,
                ContentType = message.ContentType,

            };

            if (DataReceivedAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataReceivedAsync.GetInvocationList(),
                     e => ((dDataReceivedAsync<MqttMessage>)e)(this, msg, GetRemoteEndpoint()))).ConfigureAwait(false);
            }
        }

        private async void Client_MessageReceived(ICCS.DataBroker.MQTT.ITetMqttClient client,
            ICCS.DataBroker.MQTT.MessageArgs message)
        {
            MqttMessage msg = new MqttMessage()
            {
                Payload = message.Message,
                PayloadRaw=message.MessageRaw,
                Qos = message.QosLevel,
                Responsetopic = message.ResponseTopic,
                Retain = message.Retain,
                Topic = message.Topic,
                ContentType = message.ContentType,

            };

            if (DataReceivedAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataReceivedAsync.GetInvocationList(),
                     e => ((dDataReceivedAsync<MqttMessage>)e)(this, msg, GetRemoteEndpoint()))).ConfigureAwait(false);
            }

        }

        private async void Client_Disconnected(ICCS.DataBroker.MQTT.ITetMqttClient client)
        {
            if (DataTransportUnavailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportUnavailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
        }

        private async void Client_Connected(ICCS.DataBroker.MQTT.ITetMqttClient client)
        {
            await this.client.SubscribeAsync(this.topics).ConfigureAwait(false);

            if (DataTransportAvailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportAvailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
        }

        public Task<bool> SendAsync(MqttMessage data)
        {
            if (!client.IsConnected) return Task.FromResult(false);

            return client.PublishAsync(new ICCS.DataBroker.MQTT.TopicInfo() { Topic = data.Topic, QosLevel = data.Qos },
                 data.PayloadRaw, data.Retain, contentType: data.ContentType, responseTopic: data.Responsetopic, userPropertyInfo: data.Userproperties);
        }

        public Task<bool> SendAsync(MqttMessage data, DataEndpoint to)
        {
            return SendAsync(data);
        }

        bool enabled;
        public void Enable()
        {
            if(enabled)return; enabled = true;
            client.Enable();
        }

        public void Disable()
        {
            if (!enabled) return; enabled = false;
            client.Disable();
        }

        MqttDataEndpoint endpoint;
        public DataEndpoint GetRemoteEndpoint()
        {
            if (endpoint == null) endpoint = new MqttDataEndpoint();
            endpoint.brokerip = client.Host;
            endpoint.port = this.client.Port;
            return endpoint;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public Task<bool> SubscribeAsync(List<MqttTopic> subscribe)
        {
            List<TopicInfo> topicInfos = new List<TopicInfo>();
            foreach(var intopic in subscribe) {
                topicInfos.Add(new TopicInfo() { QosLevel = intopic.Qos, Topic = intopic.Topic });
            }

            client.Subscribe(topicInfos);

            return Task.FromResult(true);
        }
    }
}