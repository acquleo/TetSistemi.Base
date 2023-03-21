using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetSistemi.Base.Interfaces;
using TetSistemi.Base.Logger;

namespace ICCS.DataBroker.MQTT
{
    public class TetMqttClient : ITetMqttClient
    {
        bool enabled;
        TetMqttClientConfig cfg;        
        IMessageLog logger;
        Encoding encoding;
        IManagedMqttClient client;
        MQTTnet.MqttFactory factory = new MQTTnet.MqttFactory();

        bool connected = false;
        public string ClientId => this.cfg.clientid;

        public string Host => this.cfg.host;

        public int Port => this.cfg.port;

        public bool IsConnected
        {
            get
            {
                if (client == null) return false;

                return this.client.IsConnected;
            }
        }

        public event dClientEvent Connected;
        public event dClientEvent Disconnected;
        public event dClientMessageEvent MessageReceived;

        public TetMqttClient(TetMqttClientConfig cfg)
        {
            this.cfg = cfg;
            
            this.client = factory.CreateManagedMqttClient();            
            this.client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;
            this.client.ConnectedAsync += Client_ConnectedAsync;
            this.client.DisconnectedAsync += Client_DisconnectedAsync;
            
            if (this.cfg.clientid==String.Empty) this.cfg.clientid=Guid.NewGuid().ToString();

            this.logger = NLogMessageLogger.GetLogger(this);
            this.encoding = System.Text.Encoding.GetEncoding(cfg.encoding);
        }


        public async void Disable()
        {
            if (!enabled) return;
                      
            
            await client.StopAsync().ConfigureAwait(false);

            //task2.Wait(TimeSpan.FromSeconds(2));

            this.logger.Log(LogLevels.Info, this, $@"Disabled MQTT client {this.cfg.clientid} host {this.cfg.host} port {this.cfg.port}");

            enabled = false;

        }

        public async void Enable()
        {
            if (enabled) return;
            enabled = true;

            var id = this.cfg.clientid;//.Replace("-", "");
                       
            var cli_options = factory.CreateClientOptionsBuilder();
            cli_options = cli_options.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500);
            cli_options = cli_options.WithClientId(this.cfg.clientid).WithTcpServer(this.cfg.host, this.cfg.port);
            if (this.cfg.username != String.Empty)
            {
                cli_options= cli_options.WithCredentials(this.cfg.username, this.cfg.password);
            }

            var managed_options = new ManagedMqttClientOptionsBuilder();
            managed_options.WithClientOptions(cli_options)
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithMaxPendingMessages(int.MaxValue)
                .WithMaxTopicFiltersInSubscribeUnsubscribePackets(int.MaxValue)
                .WithPendingMessagesOverflowStrategy(MQTTnet.Server.MqttPendingMessagesOverflowStrategy.DropOldestQueuedMessage);

            await this.client.StartAsync(managed_options.Build()).ConfigureAwait(false);
            //task.Wait(TimeSpan.FromSeconds(2));

            this.logger.Log(LogLevels.Info, this, $@"Enabled MQTT client {this.cfg.clientid} host {this.cfg.host} port {this.cfg.port}");
        }
        
        public void Initialize()
        {
            
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public MQTTnet.Protocol.MqttQualityOfServiceLevel getLevel(byte num)
        {
            if (num <= 0) return MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce;
            if (num == 1) return MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce;
            return MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce;
        }

        public byte getLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel en)
        {
            if (en == MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce) return 0;
            if (en == MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce) return 1;
            return 2;
        }

        public async void Subscribe(List<TopicInfo> topics)
        {
            await this.SubscribeAsync(topics).ConfigureAwait(false);
        }

        public Task SubscribeAsync(List<TopicInfo> topics)
        {
            if (!this.enabled) throw new NotEnabledException(string.Empty);

            List<MqttTopicFilter> filters = new List<MqttTopicFilter>();

            foreach (var topic in topics)
            {
                filters.Add(new MqttTopicFilter()
                {
                    QualityOfServiceLevel = getLevel(topic.QosLevel),
                    RetainAsPublished = false,
                    RetainHandling = MQTTnet.Protocol.MqttRetainHandling.SendAtSubscribe,
                    Topic = topic.Topic
                });

                this.logger.Log(LogLevels.Info, this, $@"MQTT client {this.cfg.clientid} host {this.cfg.host} port {this.cfg.port} subscription {topic.Topic} qos {topic.QosLevel}");
            }
            try
            {
                return this.client.SubscribeAsync(filters);
            }
            catch(ObjectDisposedException)
            {
                return Task.CompletedTask;
            }
        }

        public Task<bool> PublishAsync(TopicInfo topic, string message, bool retain,
            string contentType = null, string responseTopic=null, List<UserPropertyInfo> userPropertyInfo = null)
        {
            if (!this.enabled) throw new NotEnabledException(string.Empty);

            if (this.client == null) throw new NotEnabledException(string.Empty);

            if (!this.client.IsConnected) return Task.FromResult(false);

            var message_byte = encoding.GetBytes(message);

            return PublishAsync(topic, message_byte, retain, contentType, responseTopic, userPropertyInfo);           
        }

        public async Task<bool> PublishAsync(TopicInfo topic, byte[] message, bool retain,
            string contentType = null, string responseTopic = null, List<UserPropertyInfo> userPropertyInfo = null)
        {
            if (!this.enabled) throw new NotEnabledException(string.Empty);

            if (this.client == null) throw new NotEnabledException(string.Empty);

            if (!this.client.IsConnected) return false;

            var message_byte = message;

            MqttApplicationMessage msg = new MqttApplicationMessage()
            {
                Payload = message_byte,
                Retain = retain,
                Topic = topic.Topic,
                QualityOfServiceLevel = getLevel(topic.QosLevel)
            };

            if (responseTopic != null)
            {
                msg.ResponseTopic = responseTopic;
            }
            if (contentType != null)
            {
                msg.ContentType = contentType;
            }
            if (userPropertyInfo != null && userPropertyInfo.Count > 0)
            {
                msg.UserProperties = new List<MqttUserProperty>();

                foreach (var pro in userPropertyInfo)
                {
                    msg.UserProperties.Add(new MqttUserProperty(pro.Name, pro.Value));
                }
            }
            await this.client.EnqueueAsync(msg).ConfigureAwait(false);

            this.logger.Log(LogLevels.Trace, this, $@"MQTT client {this.cfg.clientid} host {this.cfg.host} port {this.cfg.port} publish {topic.Topic} qos {topic.QosLevel} ");

            return true;
        }

        public void Dispose()
        {
            this.Disable();

            this.client.Dispose();
        }

        private Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            if (this.MessageReceived != null)
            {
                var msg = new MessageArgs()
                {
                    DupFlag = arg.ApplicationMessage.Dup,
                    QosLevel = getLevel(arg.ApplicationMessage.QualityOfServiceLevel),
                    Retain = arg.ApplicationMessage.Retain,
                    Topic = arg.ApplicationMessage.Topic,
                    ContentType = arg.ApplicationMessage.ContentType,
                    ResponseTopic = arg.ApplicationMessage.ResponseTopic,
                    MessageRaw=arg.ApplicationMessage.Payload
                };

                if(this.cfg.encode_payload)
                {
                    msg.Message = encoding.GetString(arg.ApplicationMessage.Payload);
                    
                }

                this.MessageReceived?.Invoke(this, msg);
            }

            return Task.CompletedTask;
        }

        private Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            if (!connected) return Task.CompletedTask;

            connected = false;

            this.Disconnected?.Invoke(this);

            return Task.CompletedTask;
        }

        private async Task Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            if (connected) return;
            connected = true;

            this.Connected?.Invoke(this);

            return;

        }
    }
}
