using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using TetSistemi.Protocol.Transport.Data.Endpoint;
using ICCS.DataBroker.MQTT;
using System.Collections.Generic;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using MQTTnet.Server;
using MQTTnet;
using System.Threading;
using System.Net.NetworkInformation;

namespace TetSistemi.Protocol.Transport.Mqtt.Tcpip
{

    public class AutoBrokerMqttDataClient : IMqttDataTransport, IEnabler, IDisposable
    {
        ICCS.DataBroker.MQTT.TetMqttClient client;

        List<TopicInfo> topics;
        MqttServer mqttServer;
        Thread spawnerThread;
        /// <summary>
        /// 
        /// </summary>
        public event dDataReceivedAsync<MqttMessage> DataReceivedAsync;
        public event dDataTransportAsync<MqttMessage> DataTransportAvailable;
        public event dDataTransportAsync<MqttMessage> DataTransportUnavailable;
        public event dDataTransportTraceAsync<MqttMessage> DataTransportTraceAsync;
        //TetSistemi.Commons.Collections.ProdConsQueueEx<ICCS.DataBroker.MQTT.MessageArgs> receivequeue;
        //TetSistemi.Commons.Collections.ProdConsQueueEx<MqttMessage> sendqueue;
        public bool Available => client.IsConnected;

        bool IsAvailable()
        {
            int port = 1883; //<--- This is your value
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (var tcpi in tcpConnInfoArray)
            {
                if (tcpi.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        public AutoBrokerMqttDataClient(ICCS.DataBroker.MQTT.TetMqttClientConfig cfg, List<TopicInfo> topics)
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883);

            mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
            

            this.client = new ICCS.DataBroker.MQTT.TetMqttClient(cfg);
            this.client.Connected += Client_Connected;
            this.client.Disconnected += Client_Disconnected;
            this.client.MessageReceived += Client_MessageReceived;
            this.topics = topics;            

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

        async void dequeuesend(MqttMessage data)
        {
            await client.PublishAsync(new ICCS.DataBroker.MQTT.TopicInfo() { Topic = data.Topic, QosLevel = data.Qos },
                 data.PayloadRaw, data.Retain, contentType: data.ContentType, responseTopic: data.Responsetopic, userPropertyInfo: data.Userproperties).ConfigureAwait(false);
        }

        public Task<bool> SendAsync(MqttMessage data, DataEndpoint to)
        {
            return SendAsync(data);
        }

        async void MqttServerSpawner()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        if (!mqttServer.IsStarted && IsAvailable())
                            await mqttServer.StartAsync().ConfigureAwait(false);
                    }
                    catch (SocketException)
                    {
                        await mqttServer.StopAsync().ConfigureAwait(false);
                    }
                    catch (ObjectDisposedException)
                    {

                    }

                    Thread.Sleep(5000);
                }
            }
            catch (ThreadInterruptedException) { }
            catch (ThreadAbortException) { }
        }

        bool enabled;
        public void Enable()
        {
            if(enabled)return; enabled = true;
            client.Enable();

            spawnerThread = new Thread(new ThreadStart(MqttServerSpawner));
            spawnerThread.IsBackground = true;
            spawnerThread.Start();

        }

        public void Disable()
        {
            if (!enabled) return; enabled = false;

            spawnerThread.Interrupt();

            
            client.Disable();

            mqttServer.StopAsync().Wait();

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
            this.Disable();

            client.Dispose();

            mqttServer.Dispose();
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public Task<bool> SubscribeAsync(List<MqttTopic> subscribe)
        {
            List<TopicInfo> topicInfos = new List<TopicInfo>();
            foreach (var intopic in subscribe)
            {
                topicInfos.Add(new TopicInfo() { QosLevel = intopic.Qos, Topic = intopic.Topic });
            }

            client.Subscribe(topicInfos);

            return Task.FromResult(true);
        }
    }
}