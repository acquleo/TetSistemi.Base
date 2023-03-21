using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using TetSistemi.Base.Interfaces;
using TetSistemi.Base.Logger;
using static LaunchDarkly.Logging.LogCapture;

namespace ICCS.DataBroker.EventSource
{
    public class DataBrokerEventSourceRaw : IDataBrokerEventSourceRaw, IEnabler, IDisposable
    {
        DataBrokerEventSourceConfig cfg;
        bool enabled=false;
        IMessageLog logger;
        LaunchDarkly.EventSource.EventSource evsrc;
        public DataBrokerEventSourceRaw(DataBrokerEventSourceConfig cfg)
        {
            this.logger = NLogMessageLogger.GetLogger(this);
            this.cfg = cfg;
        }

        public bool SseIsConnected
        {
            get;
            private set;
        }

        public bool MqttIsConnected
        {
            get;
            private set;
        }

        public event dDataBrokerEventSourceMessageReceiveRaw MessageReceive;
        public event dDataBrokerEventSourceRaw SseConnected;
        public event dDataBrokerEventSourceRaw SseDisconnected;
        public event dDataBrokerEventSourceRaw MqttConnected;
        public event dDataBrokerEventSourceRaw MqttDisconnected;
        public event dDataBrokerOnMessageRequestRaw OnHttpMessageRequest;

        public void Disable()
        {
            if (!enabled) return;

            enabled = false;
        }

        public void Enable()
        {
            if (enabled) return;

            Uri ev_url = new Uri(new Uri(this.cfg.BaseUrl), @"/sse-endpoint-ex");
            string baseurl = ev_url.ToString();
            //string baseurl = "http://localhost:5050/sse-endpoint";
            List<KeyValuePair<string, string>> topics = new List<KeyValuePair<string, string>>();

            foreach (var topic in this.cfg.Topics)
            {
                topics.Add(new KeyValuePair<string, string>("topic", $@"QOS:{topic.Qos}{topic.Topic}"));
                
            }
            IEnumerable<KeyValuePair<string, string>> ff = topics.AsEnumerable();
            baseurl += QueryHelpers.AddQueryString(ev_url.Query, ff);
           
            Uri ev_url_with_param = new Uri(baseurl);

            var cfg = LaunchDarkly.EventSource.Configuration.Builder(ev_url_with_param)                
                .ReadTimeout(this.cfg.KeepAliveTimeout)
                .ResponseStartTimeout(TimeSpan.FromSeconds(10))
                .HttpRequestModifier(Evsrc_OnHttpRequest)
                .HttpMessageHandler(this.cfg.CustomHttpMessageHandler);

            evsrc = new LaunchDarkly.EventSource.EventSource(cfg.Build());
            evsrc.MessageReceived += Evsrc_MessageReceived;
            evsrc.Error += Evsrc_Error;
            evsrc.Opened += Evsrc_Opened;
            evsrc.Closed += Evsrc_Closed;
            evsrc.StartAsync();

            enabled =true;
        }

        private void Evsrc_OnHttpRequest(HttpRequestMessage e)
        {
            this.OnHttpMessageRequest?.Invoke(this, e);
        }

        private void Evsrc_Closed(object sender, LaunchDarkly.EventSource.StateChangedEventArgs e)
        {
            this.logger.Log(LogLevels.Info, this, $@"{this.cfg.BaseUrl} {e.ReadyState}");
            Debug.WriteLine("EV CLOSED");

            if (!this.SseIsConnected) return;

            this.SseIsConnected = false;

            SseDisconnected?.Invoke(this);

            this.MqttIsConnected = false;

            MqttDisconnected?.Invoke(this);
        }

        private void Evsrc_Opened(object sender, LaunchDarkly.EventSource.StateChangedEventArgs e)
        {
            this.logger.Log(LogLevels.Info, this, $@"{this.cfg.BaseUrl} {e.ReadyState}");
            Debug.WriteLine("EV OPENED");

            if (this.SseIsConnected) return;

            this.SseIsConnected = true;

            SseConnected?.Invoke(this);
        }

        private void Evsrc_Error(object sender, LaunchDarkly.EventSource.ExceptionEventArgs e)
        {
            this.logger.Log(LogLevels.Error, this, $@"{this.cfg.BaseUrl} ERROR {e.Exception.Message}");
            Debug.WriteLine("EV ERROR " + e.Exception.Message);
        }

        void ProcessTopic(LaunchDarkly.EventSource.MessageReceivedEventArgs e)
        {
            var mqtt_message = JsonConvert.DeserializeObject(e.Message.Data, typeof(mqtt_message)) as mqtt_message; 

            MessageReceive?.Invoke(this, mqtt_message.topic, mqtt_message.retain, mqtt_message.data,
                mqtt_message.responsetopic, mqtt_message.content_type, mqtt_message.qos);
        }

        private void Evsrc_MessageReceived(object sender, LaunchDarkly.EventSource.MessageReceivedEventArgs e)
        {
            this.logger.Log(LogLevels.Trace, this, $@"{this.cfg.BaseUrl} EVENT {DateTime.Now.ToString("HH:mm:ss.fff")} {e.EventName} {e.Message}");

            switch (e.EventName)
            {
                case SystemEvents.keepalive:
                {
                    break;
                }
                case SystemEvents.mqtt_disconnected:
                    {
                        if (this.MqttIsConnected)
                        {
                            this.MqttIsConnected = false;

                            MqttDisconnected?.Invoke(this);
                        }
                        break;
                    }
                case SystemEvents.mqtt_connected:
                    {
                        if (!this.MqttIsConnected)
                        {
                            this.MqttIsConnected = true;

                            MqttConnected?.Invoke(this);
                        }
                        break;
                    }
                case SystemEvents.mqtt_message:
                    {
                        ProcessTopic(e);
                        break;
                    }                
            }

        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public void Dispose()
        {
            this.Disable();

            this.evsrc?.Dispose();
        }
    }
}
