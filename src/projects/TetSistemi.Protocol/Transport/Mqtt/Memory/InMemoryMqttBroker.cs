using ICCS.DataBroker.MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetSistemi.Protocol.Transport.Mqtt.Memory
{
    public class InMemoryMqttBroker
    {
        static object intancelock = new object();
        static InMemoryMqttBroker instance;
        public static InMemoryMqttBroker Instance
        {
            get
            {
                lock(intancelock)
                {
                    if(instance == null)
                        instance = new InMemoryMqttBroker();
                    return instance;
                }
            }
        }

        internal class InMemoryMqttBrokerSubscriberTopic
        {
            public TopicInfo topic { get; set; }
            public InMemoryMqttBrokerClient client { get; set; }
        }

        public class InMemoryMqttBrokerMessage
        {
            public TopicInfo topic { get; set; }
            public byte[] message { get; set; }
            public bool Retain { get; set; }
            public string ContentType { get; set; }
            public string ResponseTopic { get; set; }
            public List<UserPropertyInfo> UserProperties { get; set; }
        }

        List<InMemoryMqttBrokerSubscriberTopic> subscribers = new List<InMemoryMqttBrokerSubscriberTopic>();
        Dictionary<string, InMemoryMqttBrokerMessage> messages = new Dictionary<string, InMemoryMqttBrokerMessage>();

        internal InMemoryMqttBroker()
        {

        }

        public void NotifyRetainMessages(InMemoryMqttBrokerClient client, TopicInfo topics)
        {
            List<InMemoryMqttBrokerMessage> toBeNotified = new List<InMemoryMqttBrokerMessage> { };
            lock (messages)
            {
                foreach (var msg in messages)
                {
                    if(InMemoryMqttTopicFilterComparer.Compare(msg.Key, topics.Topic)== InMemoryMqttTopicFilterCompareResult.IsMatch)
                    {
                        toBeNotified.Add(msg.Value);
                    }
                }
            }


            foreach(var msg in toBeNotified)
            {
                client.PushMessage(msg);
            }

        }

        public void Subscribe(InMemoryMqttBrokerClient client, List<TopicInfo> topics)
        {
            lock (subscribers)
            {
                foreach (var topic in topics)
                {
                    subscribers.Add(new InMemoryMqttBrokerSubscriberTopic() { client = client, topic = topic });

                    new TaskFactory().StartNew(() => { NotifyRetainMessages(client, topic); });
                }
            }

            
        }

        public void Publish(InMemoryMqttBrokerMessage message)
        {
            if (message.Retain)
            {
                lock (messages)
                {
                    if(!messages.ContainsKey(message.topic.Topic))
                        messages.Add(message.topic.Topic, message);
                    else
                        messages[message.topic.Topic] = message;
                }
            }

            new TaskFactory().StartNew(() => { PublishToSubscribers(message); });
            
        }

        void PublishToSubscribers(InMemoryMqttBrokerMessage message)
        {
            List<InMemoryMqttBrokerSubscriberTopic> link;
            lock (subscribers)
            {
                link = subscribers.ToList();
            }

            foreach (var topic in link) { 
                if(InMemoryMqttTopicFilterComparer.Compare(message.topic.Topic, topic.topic.Topic)== InMemoryMqttTopicFilterCompareResult.IsMatch)
                {
                    topic.client.PushMessage(message);
                }
            
            }
        }

    }
}
