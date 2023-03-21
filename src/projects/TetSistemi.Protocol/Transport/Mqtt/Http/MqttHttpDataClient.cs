using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using ICCS.DataBroker.MQTT;
using ICCS.DataBroker.EventSource;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;

namespace TetSistemi.Protocol.Transport.Mqtt.Tcpip
{
    /// <summary>
    /// Implements an mqtt data transport via HTTP interface
    /// </summary>
    public class MqttHttpDataClient : IDataTransport<MqttMessage>, IDisposable, IEnabler
    {
        ICCS.DataBroker.RestClient.BrokerClientEx.Client client;
        ICCS.DataBroker.EventSource.DataBrokerEventSourceRaw eventsource;

        List<TopicInfo> topics;

        public event dDataReceivedAsync<MqttMessage> DataReceivedAsync;
        public event dDataTransportAsync<MqttMessage> DataTransportAvailable;
        public event dDataTransportAsync<MqttMessage> DataTransportUnavailable;
        public event dDataTransportTraceAsync<MqttMessage> DataTransportTraceAsync;
        public bool Available => eventsource.MqttIsConnected;

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="baseurl">http broker url</param>
        /// <param name="cfg">configuration</param>
        /// <param name="topics">subscription topics</param>
        /// <param name="httpClient">http client (optional)</param>
        public MqttHttpDataClient(string baseurl, ICCS.DataBroker.MQTT.TetMqttClientConfig cfg, List<TopicInfo> topics
            , HttpClient httpClient = null)
        {
            //var httpClientHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            if (httpClient==null) httpClient=new HttpClient();
            
            var icfg = new ICCS.DataBroker.EventSource.DataBrokerEventSourceConfig()
            {
                BaseUrl = baseurl,
                KeepAliveTimeout = TimeSpan.FromSeconds(30),

               
            };
            icfg = new ICCS.DataBroker.EventSource.DataBrokerEventSourceConfig();
            icfg.BaseUrl=baseurl;
            foreach (var topic in topics)
            {
                icfg.Topics.Add(new ICCS.DataBroker.EventSource.DataBrokerTopicInfo()
                {
                    Topic = topic.Topic,
                    Qos=topic.QosLevel
                });
            }

            //icfg.CustomHttpMessageHandler = httpClientHandler;

            this.eventsource = new DataBrokerEventSourceRaw(icfg);
            this.client = new ICCS.DataBroker.RestClient.BrokerClientEx.Client(baseurl, httpClient);
            this.eventsource.MqttConnected += Eventsource_MqttConnected;
            this.eventsource.MqttDisconnected += Eventsource_MqttDisconnected;
            this.eventsource.SseDisconnected += Eventsource_SseDisconnected;
            this.eventsource.MessageReceive += Eventsource_MessageReceive;
            this.topics = topics;
        }

        private async void Eventsource_SseDisconnected(IDataBrokerEventSourceRaw source)
        {
            if (DataTransportUnavailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportUnavailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
        }

        private async void Eventsource_MessageReceive(IDataBrokerEventSourceRaw source, string topic, bool retain, string message, string responsetopic
        , string contenttype, byte qos)
        {
            MqttMessage msg = new MqttMessage()
            {
                Payload = message,
                Qos = qos,
                Responsetopic = responsetopic,
                Retain = retain,
                Topic = topic,
                ContentType = contenttype,

            };

            if (DataReceivedAsync != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataReceivedAsync.GetInvocationList(),
                     e => ((dDataReceivedAsync<MqttMessage>)e)(this, msg, GetRemoteEndpoint()))).ConfigureAwait(false);
            }
        }

        private async void Eventsource_MqttDisconnected(ICCS.DataBroker.EventSource.IDataBrokerEventSourceRaw source)
        {
            if (DataTransportUnavailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportUnavailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
        }

        private async void Eventsource_MqttConnected(ICCS.DataBroker.EventSource.IDataBrokerEventSourceRaw source)
        {
            if (DataTransportAvailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportAvailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
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

            //this.receivequeue.Enqueue(message);
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
            if (DataTransportAvailable != null)
            {
                await Task.WhenAll(Array.ConvertAll(
                     DataTransportAvailable.GetInvocationList(),
                     e => ((dDataTransportAsync<MqttMessage>)e)(this))).ConfigureAwait(false);
            }
        }

        public Task<bool> SendAsync(MqttMessage data)
        {
            if (!eventsource.MqttIsConnected) return Task.FromResult(false);

            return client.MqttPublishExAsync(new ICCS.DataBroker.RestClient.BrokerClientEx.Data()
                {
                    Message = data.Payload,
                    ContentType=data.ContentType,
                    Qos=data.Qos,
                    ResponseTopic=data.Responsetopic,
                    Retain=data.Retain,
                    Topic=data.Topic
                });
        }

        public Task<bool> SendAsync(MqttMessage data, DataEndpoint to)
        {
            return SendAsync(data);
        }

        public bool IsEnabled()
        {
            return eventsource.IsEnabled();
        }

        public void Enable()
        {
            eventsource.Enable();
        }

        public void Disable()
        {
            eventsource.Disable();
        }

        HttpDataEndpoint endpoint;
        public DataEndpoint GetRemoteEndpoint()
        {
            if(endpoint== null) endpoint= new HttpDataEndpoint();
            endpoint.url = client.BaseUrl;
            return endpoint;
        }

        public void Dispose()
        {
            eventsource.Dispose();
        }
    }
}